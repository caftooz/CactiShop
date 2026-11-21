namespace CactiShop.Contracts.Orders;

public record OrderProductsDto(
    string ProductName,
    decimal ProductPrice,
    int ProductQuantity
    );
