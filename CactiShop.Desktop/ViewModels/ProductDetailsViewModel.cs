using System.Windows.Media;
using CactiShop.Desktop.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CactiShop.Desktop.ViewModels;

public partial class ProductDetailsViewModel(
    ICatalogService _catalogService,
    INavigationService _navigationService,
    IImageService _imageService
    ) : PageViewModel
{

    [ObservableProperty]
    private bool _hasModel;

    public override Task OnNavigatedToAsync(object? parameter) => RunAsync(async () =>
    {
        if (parameter is not int id)
            throw new ArgumentException("Карточке товара нужен id товара.");

        var model = await _catalogService.GetProductAsync(id);
        HasModel = model != null;

        if (model is not null)
        {
            Name = model.Name;
            CategoryName = model.CategoryName;
            PriceText = $"{model.Price:0.00} ₽";
            AvgRatingText = model.AvgRating == null ? "Нет оценок" : $"★ {model.AvgRating:0.0}";
            Description = model.Description;
        }


        Image = await _imageService.GetImageAsync(model?.ImageUrl);
    });

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _categoryName;

    [ObservableProperty]
    private string _priceText;

    [ObservableProperty]
    private string _avgRatingText;

    [ObservableProperty]
    private string _description;

    [ObservableProperty]
    private ImageSource? _image;

    [RelayCommand]
    private void GoBack() => _navigationService.GoBack();
}
