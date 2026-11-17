using System;
using System.Collections.Generic;
using System.Text;

namespace CactiShop.Contracts.Orders;

public record OrderProductsDto(
    string ProductName,
    decimal ProductPrice,
    int ProductQuantity
    );
