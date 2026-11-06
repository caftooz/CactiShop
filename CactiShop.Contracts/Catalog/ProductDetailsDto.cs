namespace CactiShop.Contracts.Catalog;

/// <summary>
///     Товар для карточки: всё из списка плюс описание.
/// </summary>
public record ProductDetailDto(ProductListItemDto Item, string Description);