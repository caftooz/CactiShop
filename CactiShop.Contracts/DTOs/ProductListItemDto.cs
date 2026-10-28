using System;
using System.Collections.Generic;
using System.Text;

namespace CactiShop.Contracts.DTOs;

public record ProductListItemDto
(
    int Id,
    string Name,
    decimal Price,
    string CategoryName,
    double? AverageRating,
    int ReviewsCount,
    int? PackSize,
    int StockQuantity,
    bool IsAvailable,
    bool CanBuy
);
