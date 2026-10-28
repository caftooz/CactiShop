using CactiShop.Contracts.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CactiShop.Desktop.Services.Interfaces;

public interface ICatalogService
{
    Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken ct = default);

    Task<IReadOnlyList<ProductListItemDto>> GetProductsAsync(ProductFilterDto filter, CancellationToken ct = default);

    Task<ProductDetailsDto> GetProductAsync(int id, CancellationToken ct = default);
}
