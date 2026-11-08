using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using CactiShop.Desktop.Services.Interfaces;

namespace CactiShop.Desktop.Services;

public class ImageService(Uri _baseAddress) : IImageService
{
    private Uri? Resolve(string? relativeUrl) =>
        !string.IsNullOrEmpty(relativeUrl) && Uri.TryCreate(_baseAddress, relativeUrl, out var uri)
            ? uri
            : null;
    private BitmapImage? LoadImage(Uri? uri)
    {
        if (uri is null) return null;

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
            return null;
        }
    }

    private BitmapImage _noImage;
    private BitmapImage NoImage
    {
        get => _noImage ??= Application.Current.Resources["NoImage"] as BitmapImage;
    }

    public BitmapImage GetImage(string? relativeUrl) => LoadImage(Resolve(relativeUrl)) ?? NoImage;
}
