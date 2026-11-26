using System.Windows.Media;
using CactiShop.Desktop.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CactiShop.Desktop.ViewModels;

public partial class ProductDetailsViewModel(
    ICatalogService _catalogService,
    INavigationService _navigationService,
    IImageService _imageService,
    ICartService _cartService,
    ISessionService _sessionService,
    IDialogService _dialogService
    ) : PageViewModel
{

    [ObservableProperty]
    private bool _hasModel;
    public bool HasInCart => Quantity > 0;

    private int _productId;

    public override Task OnNavigatedToAsync(object? parameter) => RunAsync(async () =>
    {
        if (parameter is not int id)
            throw new ArgumentException("Карточке товара нужен id товара.");

        var model = await _catalogService.GetProductAsync(id);
        HasModel = model != null;

        if (HasModel)
        {
            _productId = model.Id;
            Name = model.Name;
            CategoryName = model.CategoryName;
            PriceText = $"{model.Price:0.00} ₽";
            AvgRatingText = model.AvgRating == null ? "Нет оценок" : $"★ {model.AvgRating:0.0}";
            Description = model.Description;
            Image = await _imageService.GetImageAsync(model.ImageUrl);
        }

        await LoadQuantityAsync();
    });

    public override async Task<bool> OnNavigatingFromAsync()
    {
        if (!_sessionService.IsLoggedIn)
            return await Task.FromResult(true);
        if (Quantity != await _cartService.GetProductQuantity(_productId))
        {
            await _cartService.SetProductQuantity(_productId, Quantity);
        }
        return await Task.FromResult(true);
    }
    public async Task LoadQuantityAsync()
    {
        if (_sessionService.IsLoggedIn)
        {
            Quantity = await _cartService.GetProductQuantity(_productId);
        }
        else
        {
            Quantity = 0;
        }
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasInCart))]
    private int _quantity;

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
    public void IncreaseQuantity() { if (Quantity < 99) Quantity++; }
    [RelayCommand]
    public void DecreaseQuantity() { if (Quantity > 0) Quantity--; }

    [RelayCommand]
    public void AddToCart()
    {
        if (!_sessionService.IsLoggedIn)
        {
            _dialogService.ShowInfo("Для добавления в корзину необходимо войти");
            return;
        }

        Quantity++;
    }

    [RelayCommand]
    private void GoBack() => _navigationService.GoBack();
}
