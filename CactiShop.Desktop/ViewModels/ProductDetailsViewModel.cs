using CactiShop.Contracts.Catalog;
using CactiShop.Desktop.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CactiShop.Desktop.ViewModels;

public partial class ProductDetailsViewModel(
    ICatalogService _catalogService, 
    INavigationService _navigationService
    ) : PageViewModel
{

    [ObservableProperty]
    private ProductDetailsDto? _product;

    public override Task OnNavigatedToAsync(object? parameter) => RunAsync(async () =>
    {
        if (parameter is not int id)
            throw new ArgumentException("Карточке товара нужен id товара.");

        Product = await _catalogService.GetProductAsync(id);
    });

    [RelayCommand]
    private void GoBack() => _navigationService.GoBack();
}
