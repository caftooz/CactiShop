using System.Windows.Media;

namespace CactiShop.Desktop.Services.Interfaces;

public interface IImageService
{
    Task<ImageSource> GetImageAsync(string? url);
}
