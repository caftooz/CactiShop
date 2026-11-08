namespace CactiShop.Desktop.Models;

public record ProductSummary(
    int Id,
    string Name,
    string CategoryName,
    decimal Price,
    string? ImageUrl,
    double? AvgRating
);
