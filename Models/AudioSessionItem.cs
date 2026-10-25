using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using NAudio.CoreAudioApi;

namespace SoundPeek.Models;

public class AudioSessionItem : INotifyPropertyChanged
{
    private float _volume;
    private bool _isMuted;
    private string _displayName = string.Empty;
    private ImageSource? _icon;
    private bool _isUpdatingInternally;

    public int ProcessId { get; set; }
    public string ProcessName { get; set; } = string.Empty;
    public AudioSessionControl? SessionControl { get; set; }

    public string DisplayName
    {
        get => _displayName;
        set { _displayName = value; OnPropertyChanged(); }
    }

    public ImageSource? Icon
    {
        get => _icon;
        set { _icon = value; OnPropertyChanged(); }
    }

    /// <summary>
    /// Volume as percentage 0 - 100
    /// </summary>
    public float Volume
    {
        get => _volume;
        set
        {
            float clamped = Math.Clamp(value, 0f, 100f);
            if (Math.Abs(_volume - clamped) > 0.01f)
            {
                _volume = clamped;
                OnPropertyChanged();
                OnPropertyChanged(nameof(VolumeText));

                if (!_isUpdatingInternally && SessionControl?.SimpleAudioVolume != null)
                {
                    try
                    {
                        SessionControl.SimpleAudioVolume.Volume = _volume / 100f;
                    }
                    catch
                    {
                        // Handle potential session invalidation gracefully
                    }
                }
            }
        }
    }

    public string VolumeText => $"{(int)Math.Round(_volume)}%";

    public bool IsMuted
    {
        get => _isMuted;
        set
        {
            if (_isMuted != value)
            {
                _isMuted = value;
                OnPropertyChanged();

                if (!_isUpdatingInternally && SessionControl?.SimpleAudioVolume != null)
                {
                    try
                    {
                        SessionControl.SimpleAudioVolume.Mute = _isMuted;
                    }
                    catch
                    {
                        // Handle potential session invalidation gracefully
                    }
                }
            }
        }
    }

    /// <summary>
    /// Updates properties triggered by external Windows audio notifications
    /// without re-triggering audio API setter
    /// </summary>
    public void UpdateFromHardware(float volumeLevel, bool isMuted)
    {
        _isUpdatingInternally = true;
        try
        {
            Volume = volumeLevel * 100f;
            IsMuted = isMuted;
        }
        finally
        {
            _isUpdatingInternally = false;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
