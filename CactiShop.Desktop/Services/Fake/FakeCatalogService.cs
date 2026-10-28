using CactiShop.Contracts.DTOs;
using CactiShop.Desktop.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CactiShop.Desktop.Services.Fake;

public class FakeCatalogService : ICatalogService
{
    private static readonly List<CategoryDto> Categories =
    [
        new(1,"Продукты" ),
        new(2,"Канцелярия" ),
        new(3,"Бытовая химия" )
    ];

    private static readonly List<ProductDetailsDto> Products =
    [
        Product(1, "Молоко 1 л", 89.90m, 1, "milk.jpg", 4.6, 14, null, 12, true, "Пастеризованное, 3,2%."),
        Product(2, "Хлеб бородинский", 64m, 1, "bread.jpg", 4.1, 5, null, 0, true, "Ржаной хлеб с кориандром."),
        Product(3, "Ручка гелевая, синяя", 35m, 2, "pen.jpg", 3.8, 9, 10, 40, true, "Продаётся упаковкой."),
        Product(4, "Тетрадь 96 листов", 120m, 2, null, null, 0, null, 25, true, "Клетка, твёрдая обложка."),
        Product(5, "Средство для посуды", 249m, 3, "dish.jpg", 4.9, 31, null, 7, true, "Лимон, 900 мл."),
        Product(6, "Стиральный порошок 6 кг", 899m, 3, "powder.jpg", 4.4, 18, null, 3, false, "Снят с продажи админом.")
    ];

    public async Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken ct = default)
    {
        await Task.Delay(200, ct);
        return Categories;
    }

    public async Task<IReadOnlyList<ProductListItemDto>> GetProductsAsync(ProductFilterDto f, CancellationToken ct = default)
    {
        await Task.Delay(400, ct);

        IEnumerable<ProductDetailsDto> q = Products;
        if (!string.IsNullOrWhiteSpace(f.Search))
            q = q.Where(p => p.Name.Contains(f.Search.Trim(), StringComparison.OrdinalIgnoreCase));
        if (f.CategoryId is { } c)
            q = q.Where(p => Categories.First(x => x.Name == p.CategoryName).Id == c);
        if (f.MinPrice is { } min)
            q = q.Where(p => p.Price >= min);
        if (f.MaxPrice is { } max)
            q = q.Where(p => p.Price <= max);

        return q.ToList<ProductListItemDto>();
    }

    public async Task<ProductDetailsDto> GetProductAsync(int id, CancellationToken ct = default)
    {
        await Task.Delay(200, ct);
        return Products.FirstOrDefault(p => p.Id == id)
               ?? throw new Api.ApiException(404, "Товар не найден.");
    }

    private static ProductDetailsDto Product(int id, string name, decimal price, int categoryId, string? image,
        double? rating, int reviews, int? packSize, int stock, bool available, string description) => new(
            id,
            name,
            price,
            Categories.First(c => c.Id == categoryId).Name,
            rating,
            reviews,
            packSize,
            stock,
            available,
            available && stock > 0,
            description
        );
}