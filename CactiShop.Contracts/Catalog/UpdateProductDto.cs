namespace CactiShop.Contracts.Catalog;

public record UpdateProductDto(
    int Id,
    string Name,
    string CategoryName,
    decimal Price,
    string? ImageUrl,
    string Description
);
