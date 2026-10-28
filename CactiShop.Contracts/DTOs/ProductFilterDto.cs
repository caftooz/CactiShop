using System;
using System.Collections.Generic;
using System.Text;

namespace CactiShop.Contracts.DTOs;

public record ProductFilterDto(
    string?     Search,
    int?        CategoryId,
    decimal?    MinPrice,
    decimal?    MaxPrice
);