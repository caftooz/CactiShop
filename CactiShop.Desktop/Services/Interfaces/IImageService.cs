using System.Windows.Media.Imaging;

namespace CactiShop.Desktop.Services.Interfaces;

public interface IImageService
{
    BitmapImage GetImage(string? relativeUrl);
}
