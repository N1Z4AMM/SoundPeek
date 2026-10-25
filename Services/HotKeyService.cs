using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace SoundPeek.Services;

public sealed class HotKeyService : IDisposable
{
    private const int WM_HOTKEY = 0x0312;
    private const uint MOD_ALT = 0x0001;
    private const uint MOD_CONTROL = 0x0002;
    private const uint MOD_NOREPEAT = 0x4000;
    private const uint VK_V = 0x56;

    private readonly int _hotKeyId = 9001;
    private IntPtr _windowHandle;
    private HwndSource? _hwndSource;
    private bool _isRegistered;

    public event Action? HotKeyPressed;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    public bool Register(IntPtr windowHandle)
    {
        if (_isRegistered) Unregister();

        _windowHandle = windowHandle;
        _hwndSource = HwndSource.FromHwnd(_windowHandle);
        _hwndSource?.AddHook(WndProc);

        // Ctrl + Alt + V
        _isRegistered = RegisterHotKey(_windowHandle, _hotKeyId, MOD_CONTROL | MOD_ALT | MOD_NOREPEAT, VK_V);
        return _isRegistered;
    }

    public void Unregister()
    {
        if (_isRegistered && _windowHandle != IntPtr.Zero)
        {
            UnregisterHotKey(_windowHandle, _hotKeyId);
            _hwndSource?.RemoveHook(WndProc);
            _isRegistered = false;
        }
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == WM_HOTKEY && wParam.ToInt32() == _hotKeyId)
        {
            HotKeyPressed?.Invoke();
            handled = true;
        }
        return IntPtr.Zero;
    }

    public void Dispose()
    {
        Unregister();
    }
}
