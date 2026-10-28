using CactiShop.Contracts.Catalog;

namespace CactiShop.Desktop.Services.Interfaces;

public interface ICatalogService
{
    Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken ct = default);

    Task<IReadOnlyList<ProductListItemDto>> GetProductsAsync(ProductQuery query, CancellationToken ct = default);

    Task<ProductDetailsDto> GetProductAsync(int id, CancellationToken ct = default);
}
