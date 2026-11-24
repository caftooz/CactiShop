namespace CactiShop.Desktop.Models;

internal record Order
(
    int Id,
    string Status,
    string? OrderPoint,
    string? DeliveryAddress,
    DateOnly Date,
    decimal TotalPrice,
    IEnumerable<OrderProduct> OrderProducts
);
