using System.Globalization;
using System.Windows.Data;

namespace DeliverySystem.Presentation.Wpf.Converters;

public sealed class BooleanToOpacityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool b)
        {
            return b ? 1d : 0d;
        }

        return 0d;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

