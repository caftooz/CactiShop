using CactiShop.Contracts.Catalog;
using CactiShop.Desktop.Models;
using CactiShop.Desktop.Services.Interfaces;

namespace CactiShop.Desktop.Services.Fake;

public class FakeCatalogService : ICatalogService
{
    public FakeCatalogService()
    {
        _categories =
        [
            new(1, "Продукты"),
            new(2, "Канцелярия"),
            new(3, "Бытовая химия"),
        ];
        _products =
        [
            NewProduct(1, "Молоко 1 л"              , 89.90m , GetCategoryName(1) , "Images/TestProducts/milk.png"   , 4.6 , "Пастеризованное, 3,2%."),
            NewProduct(2, "Хлеб бородинский"        , 64m    , GetCategoryName(1) , "Images/TestProducts/bread.png"  , 4.1 , "Ржаной хлеб с кориандром."),
            NewProduct(3, "Ручка гелевая, синяя"    , 35m    , GetCategoryName(2) , "Images/TestProducts/pen.png"    , 3.8 , "Продаётся упаковкой."),
            NewProduct(4, "Тетрадь 96 листов"       , 120m   , GetCategoryName(2) ,  null                            , null, "Клетка, твёрдая обложка."),
            NewProduct(5, "Средство для посуды"     , 249m   , GetCategoryName(3) , "Images/TestProducts/dish.png"   , 4.9 , "Лимон, 900 мл."),
            NewProduct(6, "Стиральный порошок 6 кг" , 899m   , GetCategoryName(3) , "Images/TestProducts/powder.png" , 4.4 , "Снят с продажи админом.")
        ];
    }

    private readonly List<Category> _categories;
    private readonly List<ProductDetails> _products;

    public async Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken ct = default)
    {
        await Task.Delay(200, ct); // Имитация задержки
        return _categories;
    }

    public async Task<IReadOnlyList<ProductSummary>> GetCatalogAsync(ProductQuery query, CancellationToken ct = default)
    {
        await Task.Delay(400, ct); // Имитация задержки

        IEnumerable<ProductSummary> q = _products.Select(p => new ProductSummary(p.Id, p.Name, p.CategoryName, p.Price, p.ImageUrl, p.AvgRating));

        if (!string.IsNullOrWhiteSpace(query.Query))
            q = q.Where(p => p.Name.Contains(query.Query.Trim(), StringComparison.OrdinalIgnoreCase));
        if (query.CategoryId is { } c)
            q = q.Where(p => _categories.First(x => x.Name == p.CategoryName).Id == c);
        if (query.MinPrice is { } min)
            q = q.Where(p => p.Price >= min);
        if (query.MaxPrice is { } max)
            q = q.Where(p => p.Price <= max);

        return q.ToList();
    }



    public async Task<ProductDetails> GetProductAsync(int id, CancellationToken ct = default)
    {
        await Task.Delay(200, ct); // Имитация задержки
        return _products.First(p => p.Id == id);
    }
    private string GetCategoryName(int categoryId) => _categories.First(c => c.Id == categoryId).Name;
    private static ProductDetails NewProduct
    (
        int id,
        string name,
        decimal price,
        string categoryName,
        string? imageUrl,
        double? rating,
        string description
    ) => new ProductDetails
         (
            id,
            name,
            categoryName,
            price,
            imageUrl,
            rating,
            description
         );
}
