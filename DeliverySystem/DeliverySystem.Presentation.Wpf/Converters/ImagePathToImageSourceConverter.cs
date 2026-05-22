using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace DeliverySystem.Presentation.Wpf.Converters;

public sealed class ImagePathToImageSourceConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var raw = value as string;
        if (string.IsNullOrWhiteSpace(raw)) return null;

        var first = raw.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).FirstOrDefault();
        if (string.IsNullOrWhiteSpace(first)) return null;

        try
        {
            if (Uri.TryCreate(first, UriKind.Absolute, out var uri))
            {
                return CreateBitmap(uri);
            }

            var full = Path.IsPathRooted(first)
                ? Path.GetFullPath(first)
                : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, first));
            if (!File.Exists(full)) return null;

            return CreateBitmap(new Uri(full, UriKind.Absolute));
        }
        catch
        {
            return null;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    private static BitmapImage CreateBitmap(Uri uri)
    {
        var img = new BitmapImage();
        img.BeginInit();
        img.CacheOption = BitmapCacheOption.OnLoad;
        img.UriSource = uri;
        img.EndInit();
        img.Freeze();
        return img;
    }
}
