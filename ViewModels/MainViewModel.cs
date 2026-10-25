using System.Collections.ObjectModel;
using System.Windows.Input;
using SoundPeek.Models;
using SoundPeek.Services;

namespace SoundPeek.ViewModels;

public sealed class MainViewModel : ViewModelBase
{
    private readonly IAudioService _audioService;
    private float _masterVolume;
    private bool _masterIsMuted;
    private string _deviceName = string.Empty;

    public ObservableCollection<AudioSessionItem> AudioSessions { get; } = new();

    public ICommand ToggleMasterMuteCommand { get; }
    public ICommand ToggleSessionMuteCommand { get; }
    public ICommand RefreshCommand { get; }

    public MainViewModel(IAudioService audioService)
    {
        _audioService = audioService;

        _masterVolume = _audioService.MasterVolume;
        _masterIsMuted = _audioService.MasterIsMuted;
        _deviceName = _audioService.DefaultDeviceName;

        ToggleMasterMuteCommand = new RelayCommand(() =>
        {
            MasterIsMuted = !MasterIsMuted;
        });

        ToggleSessionMuteCommand = new RelayCommand(param =>
        {
            if (param is AudioSessionItem session)
            {
                session.IsMuted = !session.IsMuted;
            }
        });

        RefreshCommand = new RelayCommand(() =>
        {
            _audioService.RefreshSessions();
        });

        _audioService.MasterVolumeChanged += (vol, muted) =>
        {
            _masterVolume = vol;
            _masterIsMuted = muted;
            OnPropertyChanged(nameof(MasterVolume));
            OnPropertyChanged(nameof(MasterVolumeText));
            OnPropertyChanged(nameof(MasterIsMuted));
        };

        _audioService.SessionsChanged += () =>
        {
            SyncSessions();
        };

        SyncSessions();
    }

    public float MasterVolume
    {
        get => _masterVolume;
        set
        {
            if (SetProperty(ref _masterVolume, Math.Clamp(value, 0f, 100f)))
            {
                OnPropertyChanged(nameof(MasterVolumeText));
                _audioService.MasterVolume = _masterVolume;
            }
        }
    }

    public string MasterVolumeText => $"{(int)Math.Round(_masterVolume)}%";

    public bool MasterIsMuted
    {
        get => _masterIsMuted;
        set
        {
            if (SetProperty(ref _masterIsMuted, value))
            {
                _audioService.MasterIsMuted = _masterIsMuted;
            }
        }
    }

    public string DeviceName
    {
        get => _deviceName;
        set => SetProperty(ref _deviceName, value);
    }

    public void OnWindowOpened()
    {
        _masterVolume = _audioService.MasterVolume;
        _masterIsMuted = _audioService.MasterIsMuted;
        _deviceName = _audioService.DefaultDeviceName;
        OnPropertyChanged(nameof(MasterVolume));
        OnPropertyChanged(nameof(MasterVolumeText));
        OnPropertyChanged(nameof(MasterIsMuted));
        OnPropertyChanged(nameof(DeviceName));

        _audioService.RefreshSessions();
    }

    private void SyncSessions()
    {
        var currentSessions = _audioService.GetActiveSessions();

        // Remove sessions that no longer exist
        for (int i = AudioSessions.Count - 1; i >= 0; i--)
        {
            if (!currentSessions.Any(s => s.ProcessId == AudioSessions[i].ProcessId))
            {
                AudioSessions.RemoveAt(i);
            }
        }

        // Add new sessions or update existing ones
        foreach (var session in currentSessions)
        {
            var existing = AudioSessions.FirstOrDefault(s => s.ProcessId == session.ProcessId);
            if (existing == null)
            {
                AudioSessions.Add(session);
            }
            else
            {
                existing.DisplayName = session.DisplayName;
                existing.Icon = session.Icon;
                existing.SessionControl = session.SessionControl;
            }
        }
    }
}
