namespace CactiShop.Contracts.Catalog;

/// <summary>
///     Параметры поиска в каталоге. null — «не фильтровать по этому полю».
///     WPF собирает их из полей ввода, API принимает из строки запроса: [FromQuery] ProductQuery query.
/// </summary>
public record ProductQuery(
    string? Query = null,
    int? CategoryId = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null);
