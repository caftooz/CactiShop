namespace CactiShop.Desktop.Models;

public record ProductDetails(
    int Id,
    string Name,
    string CategoryName,
    decimal Price,
    string? ImageUrl,
    double? AvgRating,
    string Description
    );
