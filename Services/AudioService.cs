using System.Collections.Concurrent;
using System.Diagnostics;
using System.Windows;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using SoundPeek.Models;
using Application = System.Windows.Application;

namespace SoundPeek.Services;

public sealed class AudioService : IAudioService
{
    private readonly MMDeviceEnumerator _deviceEnumerator;
    private MMDevice? _defaultPlaybackDevice;
    private readonly List<AudioSessionItem> _activeSessions = new();
    private readonly object _lock = new();
    private bool _isUpdatingMasterInternally;

    public event Action? SessionsChanged;
    public event Action<float, bool>? MasterVolumeChanged;

    public AudioService()
    {
        _deviceEnumerator = new MMDeviceEnumerator();
        InitializeDevice();
    }

    private void InitializeDevice()
    {
        try
        {
            _defaultPlaybackDevice = _deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            if (_defaultPlaybackDevice != null)
            {
                _defaultPlaybackDevice.AudioEndpointVolume.OnVolumeNotification += OnEndpointVolumeNotification;
                
                try
                {
                    _defaultPlaybackDevice.AudioSessionManager.OnSessionCreated += (s, e) =>
                    {
                        Application.Current?.Dispatcher.InvokeAsync(RefreshSessions);
                    };
                }
                catch
                {
                    // Some Windows versions/drivers may not support session creation events directly
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to initialize default audio device: {ex.Message}");
        }
    }

    private void OnEndpointVolumeNotification(AudioVolumeNotificationData data)
    {
        if (_isUpdatingMasterInternally) return;

        Application.Current?.Dispatcher.InvokeAsync(() =>
        {
            MasterVolumeChanged?.Invoke(data.MasterVolume * 100f, data.Muted);
        });
    }

    public string DefaultDeviceName
    {
        get
        {
            try
            {
                return _defaultPlaybackDevice?.FriendlyName ?? "Default Output Device";
            }
            catch
            {
                return "Default Output Device";
            }
        }
    }

    public float MasterVolume
    {
        get
        {
            try
            {
                return (_defaultPlaybackDevice?.AudioEndpointVolume.MasterVolumeLevelScalar ?? 1.0f) * 100f;
            }
            catch
            {
                return 100f;
            }
        }
        set
        {
            if (_defaultPlaybackDevice == null) return;
            _isUpdatingMasterInternally = true;
            try
            {
                float clamped = Math.Clamp(value / 100f, 0f, 1f);
                _defaultPlaybackDevice.AudioEndpointVolume.MasterVolumeLevelScalar = clamped;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error setting master volume: {ex.Message}");
            }
            finally
            {
                _isUpdatingMasterInternally = false;
            }
        }
    }

    public bool MasterIsMuted
    {
        get
        {
            try
            {
                return _defaultPlaybackDevice?.AudioEndpointVolume.Mute ?? false;
            }
            catch
            {
                return false;
            }
        }
        set
        {
            if (_defaultPlaybackDevice == null) return;
            _isUpdatingMasterInternally = true;
            try
            {
                _defaultPlaybackDevice.AudioEndpointVolume.Mute = value;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error toggling master mute: {ex.Message}");
            }
            finally
            {
                _isUpdatingMasterInternally = false;
            }
        }
    }

    public IReadOnlyList<AudioSessionItem> GetActiveSessions()
    {
        lock (_lock)
        {
            return _activeSessions.ToList();
        }
    }

    public void RefreshSessions()
    {
        if (_defaultPlaybackDevice == null)
        {
            InitializeDevice();
            if (_defaultPlaybackDevice == null) return;
        }

        try
        {
            var sessionManager = _defaultPlaybackDevice.AudioSessionManager;
            sessionManager.RefreshSessions();
            var sessions = sessionManager.Sessions;

            var newSessionList = new List<AudioSessionItem>();

            for (int i = 0; i < sessions.Count; i++)
            {
                var session = sessions[i];
                if (session == null) continue;

                int processId = (int)session.GetProcessID;
                if (processId <= 0) continue; // Skip idle / system non-process sessions

                string processName = "Application";
                try
                {
                    using var proc = Process.GetProcessById(processId);
                    processName = proc.ProcessName;
                }
                catch
                {
                    // Process may have exited or access restricted
                }

                // Check display name
                string displayName = !string.IsNullOrWhiteSpace(session.DisplayName)
                    ? session.DisplayName
                    : processName;

                float currentVolume = 100f;
                bool isMuted = false;

                if (session.SimpleAudioVolume != null)
                {
                    currentVolume = session.SimpleAudioVolume.Volume * 100f;
                    isMuted = session.SimpleAudioVolume.Mute;
                }

                var item = new AudioSessionItem
                {
                    ProcessId = processId,
                    ProcessName = processName,
                    DisplayName = displayName,
                    SessionControl = session,
                    Icon = IconExtractor.GetIconForProcess(processId)
                };

                item.UpdateFromHardware(currentVolume / 100f, isMuted);

                // Register event handler to listen to external volume changes (e.g. from volume keys or in-app changes)
                try
                {
                    session.RegisterEventClient(new AudioSessionEventsHandler(item));
                }
                catch
                {
                    // Ignore if already registered or not supported
                }

                newSessionList.Add(item);
            }

            lock (_lock)
            {
                _activeSessions.Clear();
                _activeSessions.AddRange(newSessionList);
            }

            SessionsChanged?.Invoke();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error refreshing audio sessions: {ex.Message}");
        }
    }

    public void Dispose()
    {
        if (_defaultPlaybackDevice != null)
        {
            try
            {
                _defaultPlaybackDevice.AudioEndpointVolume.OnVolumeNotification -= OnEndpointVolumeNotification;
            }
            catch { }
            _defaultPlaybackDevice.Dispose();
            _defaultPlaybackDevice = null;
        }

        _deviceEnumerator.Dispose();
    }

    private class AudioSessionEventsHandler : IAudioSessionEventsHandler
    {
        private readonly AudioSessionItem _item;

        public AudioSessionEventsHandler(AudioSessionItem item)
        {
            _item = item;
        }

        public void OnVolumeChanged(float volume, bool isMuted)
        {
            Application.Current?.Dispatcher.InvokeAsync(() =>
            {
                _item.UpdateFromHardware(volume, isMuted);
            });
        }

        public void OnDisplayNameChanged(string displayName)
        {
            Application.Current?.Dispatcher.InvokeAsync(() =>
            {
                if (!string.IsNullOrWhiteSpace(displayName))
                {
                    _item.DisplayName = displayName;
                }
            });
        }

        public void OnIconPathChanged(string iconPath) { }
        public void OnSimpleVolumeChanged(float volume, bool isMuted)
        {
            Application.Current?.Dispatcher.InvokeAsync(() =>
            {
                _item.UpdateFromHardware(volume, isMuted);
            });
        }

        public void OnChannelVolumeChanged(uint channelCount, IntPtr newVolumes, uint changedChannel) { }
        public void OnGroupingParamChanged(ref Guid groupingId) { }
        public void OnStateChanged(AudioSessionState state) { }
        public void OnSessionDisconnected(AudioSessionDisconnectReason disconnectReason) { }
    }
}
