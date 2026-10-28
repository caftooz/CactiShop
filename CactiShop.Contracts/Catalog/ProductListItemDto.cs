namespace CactiShop.Contracts.Catalog;

/// <summary>
///     Товар в списке каталога
/// </summary>
/// <param name="ImageUrl">
///     Адрес фото относительно адреса API, например "images/milk.jpg". null — фото нет.
/// </param>
/// <param name="AverageRating">
///     Средняя оценка. null — отзывов ещё нет.
/// </param>
/// <param name="CanBuy">
///     Можно ли купить прямо сейчас. Считает сервер.
/// </param>
public record ProductListItemDto(
    int     Id,
    string  Name,
    decimal Price,
    string  CategoryName,
    string? ImageUrl,
    double? AverageRating,
    int     ReviewsCount,
    int     StockQuantity,
    bool    IsAvailable,
    bool    CanBuy);
