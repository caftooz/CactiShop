using System;
using System.Collections.Generic;
using System.Text;

namespace CactiShop.Contracts.Orders;

public record OrderDto(
    int Id,
    string Status,
    string? OrderPoint,
    string? DeliveryAddress,
    DateOnly Date,
    decimal TotalPrice,
    IEnumerable<OrderProductsDto> OrderProducts
);
