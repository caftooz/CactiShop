using System.IO;
using System.Windows.Media;

namespace CactiShop.Desktop.Services.Interfaces;

public interface IImageService
{
    Task<ImageSource> GetImageAsync(string? url);
    Task<string> UploadImageAsync(string filename);
}
