using CactiShop.Contracts.Catalog;
using CactiShop.Desktop.Services.Interfaces;

namespace CactiShop.Desktop.Services.Fake;

public class FakeCatalogService : ICatalogService
{
    private static readonly List<CategoryDto> CATEGORIES =
    [
        new(1, "Продукты"),
        new(2, "Канцелярия"),
        new(3, "Бытовая химия"),
    ];
    private static readonly List<ProductDetailsDto> PRODUCTS =
    [
        NewProduct(1, "Молоко 1 л"              , 89.90m , 1 , "Images/TestProducts/milk.png"   , 4.6  , 14 , 12 , true  , "Пастеризованное, 3,2%."),
        NewProduct(2, "Хлеб бородинский"        , 64m    , 1 , "Images/TestProducts/bread.png"  , 4.1  , 5  , 0  , true  , "Ржаной хлеб с кориандром."),
        NewProduct(3, "Ручка гелевая, синяя"    , 35m    , 2 , "Images/TestProducts/pen.png"    , 3.8  , 9  , 40 , true  , "Продаётся упаковкой."),
        NewProduct(4, "Тетрадь 96 листов"       , 120m   , 2 ,  null               , null , 0  , 25 , true  , "Клетка, твёрдая обложка."),
        NewProduct(5, "Средство для посуды"     , 249m   , 3 , "Images/TestProducts/dish.png"   , 4.9  , 31 , 7  , true  , "Лимон, 900 мл."),
        NewProduct(6, "Стиральный порошок 6 кг" , 899m   , 3 , "Images/TestProducts/powder.png" , 4.4  , 18 , 3  , false , "Снят с продажи админом.")
    ];

    public async Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken ct = default)
    {
        await Task.Delay(200, ct); // Имитация задержки
        return CATEGORIES;
    }

    public async Task<IReadOnlyList<ProductListItemDto>> GetProductsAsync(ProductQuery query, CancellationToken ct = default)
    {
        await Task.Delay(400, ct); // Имитация задержки

        IEnumerable<ProductListItemDto> q = PRODUCTS.Select(p => p.Item);

        if (!string.IsNullOrWhiteSpace(query.Query))
            q = q.Where(p => p.Name.Contains(query.Query.Trim(), StringComparison.OrdinalIgnoreCase));
        if (query.CategoryId is {} c)
            q = q.Where(p => CATEGORIES.First(x => x.Name == p.CategoryName).Id == c);
        if (query.MinPrice is {} min)
            q = q.Where(p => p.Price >= min);
        if (query.MaxPrice is {} max)
            q = q.Where(p => p.Price <= max);

        return q.ToList();
    }


    public async Task<ProductDetailsDto> GetProductAsync(int id, CancellationToken ct = default)
    {
        await Task.Delay(200, ct); // Имитация задержки
        return PRODUCTS.First(p => p.Item.Id == id);
    }
    private static ProductDetailsDto NewProduct
    (
        int     id, 
        string  name, 
        decimal price, 
        int     categoryId, 
        string? imageUrl,
        double? rating, 
        int     reviews,
        int     stock, 
        bool    available, 
        string  description
    ) => new ProductDetailsDto
         (
             new ProductListItemDto
             (
                 id, 
                 name, 
                 price, 
                 CATEGORIES.First(c => c.Id == categoryId).Name, 
                 imageUrl, 
                 rating, 
                 reviews,
                 stock, 
                 available, 
                 CanBuy: available && stock > 0
             ),
             description
         );
}