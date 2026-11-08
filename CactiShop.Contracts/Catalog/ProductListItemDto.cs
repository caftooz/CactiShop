namespace CactiShop.Contracts.Catalog;

/// <summary>
///     Товар для списка каталога
/// </summary>
/// <param name="ImageUrl">
///     Адрес фото относительно адреса API, например "images/milk.jpg". null — фото нет.
/// </param>
/// <param name="AverageRating">
///     Средняя оценка. null — отзывов ещё нет.
/// </param>
public record ProductListItemDto(
    int Id,
    string Name,
    decimal Price,
    string CategoryName,
    string? ImageUrl,
    double? AverageRating,
    int StockQuantity);
