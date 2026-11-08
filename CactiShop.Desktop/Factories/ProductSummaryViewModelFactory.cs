using CactiShop.Desktop.Models;
using CactiShop.Desktop.Services.Interfaces;
using CactiShop.Desktop.ViewModels;

namespace CactiShop.Desktop.Factories;

public class ProductSummaryViewModelFactory(INavigationService nav, IImageService img)
{
    public ProductSummaryViewModel Create(ProductSummary model) => new(model, nav, img);
}
