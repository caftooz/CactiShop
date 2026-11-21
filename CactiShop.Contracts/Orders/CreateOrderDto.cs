namespace CactiShop.Contracts.Orders;

public record CreateOrderDto(
    int? OrderPointId,
    string? DeliveryAddress,
    IEnumerable<CreateOrderProductsDto> OrderProducts
);

