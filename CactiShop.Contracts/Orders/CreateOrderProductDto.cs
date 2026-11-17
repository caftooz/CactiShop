using System;
using System.Collections.Generic;
using System.Text;

namespace CactiShop.Contracts.Orders;

public record CreateOrderProductsDto(
    int ProductId,
    int ProductQuantity
    );

