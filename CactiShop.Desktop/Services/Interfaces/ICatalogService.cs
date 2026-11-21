using CactiShop.Contracts.Catalog;
using CactiShop.Desktop.Models;

namespace CactiShop.Desktop.Services.Interfaces;

public interface ICatalogService
{
    Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken ct = default);

    Task<IReadOnlyList<ProductSummary>> GetCatalogAsync(ProductQuery query, CancellationToken ct = default);

    Task<ProductDetails> GetProductAsync(int id, CancellationToken ct = default);
}
