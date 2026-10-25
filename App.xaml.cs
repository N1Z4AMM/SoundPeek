using System.Threading;
using System.Windows;
using SoundPeek.Services;
using SoundPeek.ViewModels;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace SoundPeek;

public partial class App : System.Windows.Application
{
    private Mutex? _singleInstanceMutex;
    private AudioService? _audioService;
    private HotKeyService? _hotKeyService;
    private TrayService? _trayService;
    private MainWindow? _mainWindow;

    protected override void OnStartup(StartupEventArgs e)
    {
        const string mutexName = "Global\\SoundPeek_SingleInstance_Mutex";
        _singleInstanceMutex = new Mutex(true, mutexName, out bool isNewInstance);

        if (!isNewInstance)
        {
            MessageBox.Show("SoundPeek sudah berjalan di background.\nGunakan shortcut Ctrl + Alt + V atau cek System Tray.",
                "SoundPeek", MessageBoxButton.OK, MessageBoxImage.Information);
            Shutdown();
            return;
        }

        base.OnStartup(e);

        // Initialize Services
        _audioService = new AudioService();
        _hotKeyService = new HotKeyService();
        _trayService = new TrayService();

        var mainViewModel = new MainViewModel(_audioService);
        _mainWindow = new MainWindow(mainViewModel, _hotKeyService, _trayService);

        // Initialize tray
        _trayService.Initialize();

        // Show window initially once or keep in tray ready for shortcut
        _mainWindow.ShowWindow();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _hotKeyService?.Dispose();
        _trayService?.Dispose();
        _audioService?.Dispose();

        if (_singleInstanceMutex != null)
        {
            _singleInstanceMutex.ReleaseMutex();
            _singleInstanceMutex.Dispose();
        }

        base.OnExit(e);
    }
}
