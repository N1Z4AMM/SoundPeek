using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace SoundPeek.Services;

public sealed class TrayService : IDisposable
{
    private NotifyIcon? _notifyIcon;
    public event Action? ToggleRequested;

    public void Initialize()
    {
        Icon? appIcon = null;
        string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SoundPeek.ico");

        if (File.Exists(iconPath))
        {
            appIcon = new Icon(iconPath);
        }
        else
        {
            // Fallback to system application icon
            appIcon = SystemIcons.Application;
        }

        var contextMenu = new ContextMenuStrip();
        
        var openItem = new ToolStripMenuItem("Buka SoundPeek (Ctrl+Alt+V)");
        openItem.Font = new Font(openItem.Font, System.Drawing.FontStyle.Bold);
        openItem.Click += (s, e) => ToggleRequested?.Invoke();

        var settingsItem = new ToolStripMenuItem("Pengaturan");
        settingsItem.Click += (s, e) =>
        {
            System.Windows.MessageBox.Show(
                "SoundPeek v0.1 MVP\nShortcut global: Ctrl + Alt + V\n\nPengaturan lanjutan akan hadir pada v1.0.",
                "SoundPeek Settings",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);
        };

        var exitItem = new ToolStripMenuItem("Keluar");
        exitItem.Click += (s, e) =>
        {
            _notifyIcon?.Dispose();
            Application.Current.Shutdown();
        };

        contextMenu.Items.Add(openItem);
        contextMenu.Items.Add(new ToolStripSeparator());
        contextMenu.Items.Add(settingsItem);
        contextMenu.Items.Add(new ToolStripSeparator());
        contextMenu.Items.Add(exitItem);

        _notifyIcon = new NotifyIcon
        {
            Icon = appIcon,
            Text = "SoundPeek - Volume Mixer (Ctrl+Alt+V)",
            Visible = true,
            ContextMenuStrip = contextMenu
        };

        _notifyIcon.MouseClick += (s, e) =>
        {
            if (e.Button == MouseButtons.Left)
            {
                ToggleRequested?.Invoke();
            }
        };
    }

    public void Dispose()
    {
        if (_notifyIcon != null)
        {
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
            _notifyIcon = null;
        }
    }
}
