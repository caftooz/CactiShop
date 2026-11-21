namespace CactiShop.Contracts.Orders;

public record CreateOrderProductsDto(
    int ProductId,
    int ProductQuantity
    );

