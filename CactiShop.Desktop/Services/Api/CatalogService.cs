using CactiShop.Contracts.Catalog;
using CactiShop.Desktop.Services.Interfaces;
using System.Globalization;

namespace CactiShop.Desktop.Services.Api;

public class CatalogService(ApiClient _apiClient) : ICatalogService
{
    public async Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken ct = default) =>
        await _apiClient.GetAsync<List<CategoryDto>>("api/categories", ct);

    public async Task<IReadOnlyList<ProductListItemDto>> GetProductsAsync(ProductQuery query, CancellationToken ct = default) =>
        await _apiClient.GetAsync<List<ProductListItemDto>>("api/products" + BuildQuery(query), ct);

    public Task<ProductDetailsDto> GetProductAsync(int id, CancellationToken ct = default) =>
        _apiClient.GetAsync<ProductDetailsDto>($"api/products/{id}", ct);

    private static string BuildQuery(ProductQuery f)
    {
        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(f.Search))
            parts.Add("search=" + Uri.EscapeDataString(f.Search.Trim()));
        if (f.CategoryId is { } c)
            parts.Add("categoryId=" + c);
        if (f.MinPrice is { } min)
            parts.Add("minPrice=" + min.ToString(CultureInfo.InvariantCulture));
        if (f.MaxPrice is { } max)
            parts.Add("maxPrice=" + max.ToString(CultureInfo.InvariantCulture));

        return parts.Count == 0 ? "" : "?" + string.Join("&", parts);
    }
}
