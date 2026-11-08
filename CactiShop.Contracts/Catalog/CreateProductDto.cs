namespace CactiShop.Contracts.Catalog;

public record CreateProductDto(
    string Name,
    int CategoryId,
    string Description,
    decimal Price,
    int Quantity,
    string ImagePath,
    bool IsActive
    );
