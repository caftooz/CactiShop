using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CactiShop.Desktop.Services.Interfaces;

namespace CactiShop.Desktop.Services.Api;

public class ApiClient(HttpClient _client, ISessionService _sessionService)
{
    public Task<T> GetAsync<T>(string url, CancellationToken ct = default) =>
        SendAsync<T>(new HttpRequestMessage(HttpMethod.Get, url), ct);

    public Task<T> PostAsync<T>(string url, object? body, CancellationToken ct = default) =>
        SendAsync<T>(WithBody(HttpMethod.Post, url, body), ct);

    public Task<T> PutAsync<T>(string url, object? body, CancellationToken ct = default) =>
        SendAsync<T>(WithBody(HttpMethod.Put, url, body), ct);

    public Task<T> PatchAsync<T>(string url, object? body, CancellationToken ct = default) =>
        SendAsync<T>(WithBody(HttpMethod.Patch, url, body), ct);


    public async Task<T> UploadFileAsync<T>(string url, string filePath, string fieldName = "file", CancellationToken ct = default)
    {
        await using var stream = File.OpenRead(filePath);
        var file = new StreamContent(stream);
        file.Headers.ContentType = new MediaTypeHeaderValue(ContentTypeOf(filePath));

        using var form = new MultipartFormDataContent();
        form.Add(file, fieldName, Path.GetFileName(filePath));

        using var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = form };
        return await SendAsync<T>(request, ct);
    }

    private static string ContentTypeOf(string path) => Path.GetExtension(path).ToLowerInvariant() switch
    {
        ".png" => "image/png",
        ".webp" => "image/webp",
        ".jpg" or ".jpeg" => "image/jpeg",
        _ => "application/octet-stream"
    };

    public async Task SendAsync(HttpMethod method, string url, object? body = null, CancellationToken ct = default)
    {
        using var response = await SendRawAsync(WithBody(method, url, body), ct);
    }
    private async Task<T> SendAsync<T>(HttpRequestMessage request, CancellationToken ct)
    {
        using var response = await SendRawAsync(request, ct);
        var result = await response.Content.ReadFromJsonAsync<T>(cancellationToken: ct);
        return result ?? throw new ApiException((int)response.StatusCode, "Сервер вернул пустой ответ.");
    }

    private async Task<HttpResponseMessage> SendRawAsync(HttpRequestMessage request, CancellationToken ct)
    {
        if (_sessionService.Token is { } token)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request, ct);
        if (response.IsSuccessStatusCode)
            return response;

        var message = await ReadErrorMessageAsync(response, ct);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
            _sessionService.SignOut();

        var status = (int)response.StatusCode;
        response.Dispose();
        throw new ApiException(status, message);
    }

    private static HttpRequestMessage WithBody(HttpMethod method, string url, object? body)
    {
        var request = new HttpRequestMessage(method, url);
        if (body is not null)
            request.Content = JsonContent.Create(body);
        return request;
    }

    private static async Task<string> ReadErrorMessageAsync(HttpResponseMessage response, CancellationToken ct)
    {
        try
        {
            var text = await response.Content.ReadAsStringAsync(ct);
            using var json = JsonDocument.Parse(text);
            if (json.RootElement.TryGetProperty("message", out var m) && m.GetString() is { Length: > 0 } msg)
                return msg;
        }
        catch (JsonException) { }

        return response.StatusCode switch
        {
            HttpStatusCode.Unauthorized => "Нужно войти в аккаунт.",
            HttpStatusCode.Forbidden => "Недостаточно прав.",
            HttpStatusCode.NotFound => "Не найдено.",
            _ => $"Ошибка сервера ({(int)response.StatusCode})."
        };
    }
}
