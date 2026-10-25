using SoundPeek.Models;

namespace SoundPeek.Services;

public interface IAudioService : IDisposable
{
    event Action? SessionsChanged;
    event Action<float, bool>? MasterVolumeChanged;

    IReadOnlyList<AudioSessionItem> GetActiveSessions();
    float MasterVolume { get; set; }
    bool MasterIsMuted { get; set; }
    string DefaultDeviceName { get; }

    void RefreshSessions();
}
