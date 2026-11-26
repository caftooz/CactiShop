using System.Windows.Media;
using CactiShop.Desktop.Models;
using CactiShop.Desktop.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CactiShop.Desktop.ViewModels;

public partial class ProductSummaryViewModel(
    ProductSummary _model,
    INavigationService _navService,
    IImageService _imageService,
    ICartService _cartService,
    ISessionService _sessionService,
    IDialogService _dialogService
    ) : ObservableObject, IAsyncDisposable
{

    private int _oldQuantity;
    private bool _isDataLoaded;

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        if (_isDataLoaded) return;
        _isDataLoaded = true;

        var imageTask = _imageService.GetImageAsync(_model.ImageUrl);
        var qtyTask = _sessionService.IsLoggedIn
            ? _cartService.GetProductQuantity(_model.Id)
            : Task.FromResult(0);

        await Task.WhenAll(imageTask, qtyTask).ConfigureAwait(false);

        var img = await imageTask;
        var qty = await qtyTask;

        await App.Current.Dispatcher.InvokeAsync(() =>
        {
            Image = img;
            if (qty != 0)
            {
                _oldQuantity = qty;
                Quantity = qty;
            }
        }, System.Windows.Threading.DispatcherPriority.Background);
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
    public async Task LoadQuantityAsync()
    {
        if (_sessionService.IsLoggedIn)
        {
            Quantity = await _cartService.GetProductQuantity(_model.Id); _oldQuantity = Quantity;
        }
        else
            Quantity = 0;
    }

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
    public Task OpenProductAsync() =>
        _navService.NavigateToAsync<ProductDetailsViewModel>(_model.Id);

    public async ValueTask DisposeAsync()
    {
        if (!_sessionService.IsLoggedIn)
            return;
        if (Quantity == _oldQuantity)
            return;
        if (Quantity != await _cartService.GetProductQuantity(_model.Id))
        {
            await _cartService.SetProductQuantity(_model.Id, Quantity);
        }
    }
}
