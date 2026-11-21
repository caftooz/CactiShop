using System.Globalization;
using CactiShop.Contracts;
using CactiShop.Contracts.Catalog;
using CactiShop.Desktop.Models;
using CactiShop.Desktop.Services.Interfaces;

namespace CactiShop.Desktop.Services.Api;

public class CatalogService(ApiClient _apiClient) : ICatalogService
{
    public async Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken ct = default)
    {
        var dtos = await _apiClient.GetAsync<List<CategoryDto>>(ApiRoutes.Categories.List, ct);
        return dtos.Select(d => new Category(d.Id, d.Name)).ToList();
    }

    public async Task<IReadOnlyList<ProductSummary>> GetCatalogAsync(ProductQuery query, CancellationToken ct = default)
    {
        var dtos = await _apiClient.GetAsync<List<ProductListItemDto>>(ApiRoutes.Catalog.List + BuildQuery(query), ct);
        return dtos.Select(d => new ProductSummary(d.Id, d.Name, d.CategoryName, d.Price, d.ImageUrl, d.AverageRating)).ToList();
    }

    public async Task<ProductDetails> GetProductAsync(int id, CancellationToken ct = default)
    {
        var d = await _apiClient.GetAsync<ProductDetailsDto>(ApiRoutes.Catalog.ByIdUrl(id), ct);
        return new ProductDetails(d.Item.Id, d.Item.Name, d.Item.CategoryName, d.Item.Price, d.Item.ImageUrl, d.Item.AverageRating, d.Description);
    }

    private static string BuildQuery(ProductQuery f)
    {
        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(f.Query))
            parts.Add("search=" + Uri.EscapeDataString(f.Query.Trim()));
        if (f.CategoryId is { } c)
            parts.Add("categoryId=" + c);
        if (f.MinPrice is { } min)
            parts.Add("minPrice=" + min.ToString(CultureInfo.InvariantCulture));
        if (f.MaxPrice is { } max)
            parts.Add("maxPrice=" + max.ToString(CultureInfo.InvariantCulture));

        return parts.Count == 0 ? "" : "?" + string.Join("&", parts);
    }
}
