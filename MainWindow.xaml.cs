using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using SoundPeek.Services;
using SoundPeek.ViewModels;
using MessageBox = System.Windows.MessageBox;

namespace SoundPeek;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;
    private readonly HotKeyService _hotKeyService;
    private readonly TrayService _trayService;
    private bool _isClosingExplicitly;

    public MainWindow(MainViewModel viewModel, HotKeyService hotKeyService, TrayService trayService)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _hotKeyService = hotKeyService;
        _trayService = trayService;

        DataContext = _viewModel;

        _hotKeyService.HotKeyPressed += ToggleWindow;
        _trayService.ToggleRequested += ToggleWindow;

        Deactivated += (s, e) =>
        {
            if (Visibility == Visibility.Visible)
            {
                HideWindow();
            }
        };
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        var handle = new WindowInteropHelper(this).Handle;
        _hotKeyService.Register(handle);
    }

    public void ToggleWindow()
    {
        if (Visibility == Visibility.Visible && IsActive)
        {
            HideWindow();
        }
        else
        {
            ShowWindow();
        }
    }

    public void ShowWindow()
    {
        _viewModel.OnWindowOpened();

        // Reposition at bottom right corner above taskbar
        var workArea = SystemParameters.WorkArea;
        Left = workArea.Right - Width - 16;
        Top = workArea.Bottom - Height - 16;

        Visibility = Visibility.Visible;
        Activate();
        Focus();

        // Fade in animation
        var fadeIn = new DoubleAnimation(0.0, 1.0, TimeSpan.FromMilliseconds(150));
        BeginAnimation(OpacityProperty, fadeIn);
    }

    public void HideWindow()
    {
        var fadeOut = new DoubleAnimation(1.0, 0.0, TimeSpan.FromMilliseconds(120));
        fadeOut.Completed += (s, e) =>
        {
            Visibility = Visibility.Hidden;
        };
        BeginAnimation(OpacityProperty, fadeOut);
    }

    private void OnSettingsClicked(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(
            "SoundPeek v0.1 MVP\nShortcut global: Ctrl + Alt + V\n\nPengaturan lanjutan akan hadir pada v1.0.",
            "SoundPeek Settings",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        if (!_isClosingExplicitly)
        {
            e.Cancel = true;
            HideWindow();
        }
        else
        {
            base.OnClosing(e);
        }
    }

    public void ForceClose()
    {
        _isClosingExplicitly = true;
        Close();
    }
}
