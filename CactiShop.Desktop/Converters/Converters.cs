using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace CactiShop.Desktop.Converters;

public class ImageUrlConverter : IValueConverter
{
    public Uri? BaseAddress { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string { Length: > 0 } url || BaseAddress is null)
            return NoImage();
        if (!Uri.TryCreate(BaseAddress, url, out var uri))
            return NoImage();

        try
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = uri;                       
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.DecodePixelWidth = 400;                
            image.EndInit();
            if (image.CanFreeze) image.Freeze();         
            return image;
        }
        catch (Exception ex) when (ex is IOException or NotSupportedException or UnauthorizedAccessException)
        {
            return NoImage();
        }

        BitmapImage NoImage() => Application.Current.Resources["NoImage"] as BitmapImage;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}

public class InverseBoolToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is true ? Visibility.Collapsed : Visibility.Visible;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}

public class NotNullToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is null ? Visibility.Collapsed : Visibility.Visible;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
