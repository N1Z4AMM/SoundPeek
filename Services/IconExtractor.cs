using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SoundPeek.Services;

public static class IconExtractor
{
    private static readonly ConcurrentDictionary<string, ImageSource?> Cache = new(StringComparer.OrdinalIgnoreCase);

    public static ImageSource? GetIconForProcess(int processId)
    {
        if (processId <= 0) return null;

        try
        {
            using var process = Process.GetProcessById(processId);
            string? filePath = null;

            try
            {
                filePath = process.MainModule?.FileName;
            }
            catch
            {
                // Access denied or 32/64 bit mismatch on system processes
            }

            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                return GetIconForFile(filePath);
            }
        }
        catch
        {
            // Process may have exited
        }

        return null;
    }

    public static ImageSource? GetIconForFile(string filePath)
    {
        return Cache.GetOrAdd(filePath, path =>
        {
            try
            {
                using var icon = Icon.ExtractAssociatedIcon(path);
                if (icon == null) return null;

                var bitmapSource = Imaging.CreateBitmapSourceFromHIcon(
                    icon.Handle,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());

                bitmapSource.Freeze(); // Make it cross-thread accessible
                return bitmapSource;
            }
            catch
            {
                return null;
            }
        });
    }
}
