using System.Windows.Media;
using CactiShop.Desktop.Models;
using CactiShop.Desktop.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CactiShop.Desktop.ViewModels;

public partial class ProductSummaryViewModel : ObservableObject, IAsyncDisposable
{
    private ProductSummary _model;
    private INavigationService _navService;
    private IImageService _imageService;
    private ICartService _cartService;

    private int _oldQuantity;

    public ProductSummaryViewModel(ProductSummary model, INavigationService navService, IImageService imageService, ICartService cartService)
    {
        _model = model;
        _navService = navService;
        _imageService = imageService;
        _cartService = cartService;
    }

    public async Task LoadDataAsync()
    {
        await LoadImageAsync();
        await LoadQuantityAsync();
    }

    public bool HasInCart => Quantity > 0;

    public string Name => _model.Name;
    public string CategoryName => _model.CategoryName;
    public string PriceText => $"{_model.Price:0.00} ₽";
    public string AvgRatingText => _model.AvgRating == null ? "Нет оценок" : $"★ {_model.AvgRating:0.0}";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasInCart))]
    private int _quantity;

    [ObservableProperty]
    private ImageSource? _image;
    public async Task LoadImageAsync() => Image = await _imageService.GetImageAsync(_model.ImageUrl);
    public async Task LoadQuantityAsync() { Quantity = await _cartService.GetProductQuantity(_model.Id); _oldQuantity = Quantity; }

    [RelayCommand]
    public void IncreaseQuantity() { if (Quantity < 99) Quantity++; }
    [RelayCommand]
    public void DecreaseQuantity() { if (Quantity > 0) Quantity--; }

    [RelayCommand]
    public void AddToCart() => Quantity++;

    [RelayCommand]
    public Task OpenProductAsync() =>
        _navService.NavigateToAsync<ProductDetailsViewModel>(_model.Id);

    public async ValueTask DisposeAsync()
    {
        if (Quantity == _oldQuantity)
            return;
        if (Quantity != await _cartService.GetProductQuantity(_model.Id))
        {
            await _cartService.SetProductQuantity(_model.Id, Quantity);
        }
    }
}
