using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CactiShop.Desktop.Converters;

public class MultiBooleanAndNotBooleanToVisibilityConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length >= 2 && values[0] is bool b1 && values[1] is bool b2)
        {
            return (b1 && !b2) ? Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Collapsed;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
