using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CactiShop.Desktop.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace CactiShop.Desktop.Services;

public class ImageService(IHttpClientFactory _httpFactory, ILogger<ImageService> _logger) : IImageService
{
    public const string HttpClientName = "images";
    private const int DecodeWidth = 480;

    private readonly ConcurrentDictionary<string, Lazy<Task<ImageSource>>> _cache = new();
    private static readonly ImageSource NoImage = LoadPlaceholder();

    public Task<ImageSource> GetImageAsync(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return Task.FromResult(NoImage);

        return _cache.GetOrAdd(url, u => new Lazy<Task<ImageSource>>(() => LoadAsync(u))).Value;
    }

    private async Task<ImageSource> LoadAsync(string url)
    {
        try
        {
            var http = _httpFactory.CreateClient(HttpClientName);
            var uri = new Uri(http.BaseAddress!, url);

            var bytes = uri.IsFile
                ? await File.ReadAllBytesAsync(uri.LocalPath)
                : await http.GetByteArrayAsync(uri);

            return await Task.Run(() => Decode(bytes, DecodeWidth));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Не удалось загрузить изображение {Url}", url);
            _cache.TryRemove(url, out _);
            return NoImage;
        }
    }

    private static BitmapSource Decode(byte[] bytes, int? decodeWidth)
    {
        using var stream = new MemoryStream(bytes);
        var image = new BitmapImage();
        image.BeginInit();
        image.CacheOption = BitmapCacheOption.OnLoad;
        if (decodeWidth is { } w) image.DecodePixelWidth = w;
        image.StreamSource = stream;
        image.EndInit();
        image.Freeze();
        return image;
    }

    private static ImageSource LoadPlaceholder() =>
        Decode(File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Images", "no-image.png")), null);
}
