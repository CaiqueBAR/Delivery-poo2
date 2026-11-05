using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DeliveryMarketplace.Presentation.WPF.Converters;

/// <summary>
/// Converte um valor booleano em um pincel (cor). Usado em indicadores de status.
/// </summary>
public sealed class BoolToBrushConverter : IValueConverter
{
    public Brush TrueBrush { get; set; } = Brushes.Green;
    public Brush FalseBrush { get; set; } = Brushes.Gray;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is true ? TrueBrush : FalseBrush;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => Binding.DoNothing;
}
