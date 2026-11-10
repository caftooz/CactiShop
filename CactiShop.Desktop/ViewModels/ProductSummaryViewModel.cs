using System.Windows.Media;
using CactiShop.Desktop.Models;
using CactiShop.Desktop.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CactiShop.Desktop.ViewModels;

public partial class ProductSummaryViewModel : ObservableObject
{
    private ProductSummary _model;
    private INavigationService _navService;
    private IImageService _imageService;

    public ProductSummaryViewModel(ProductSummary model, INavigationService navService, IImageService imageService)
    {
        _model = model;
        _navService = navService;
        _imageService = imageService;
        _ = LoadImageAsync();
    }

    public string Name => _model.Name;
    public string CategoryName => _model.CategoryName;
    public string PriceText => $"{_model.Price:0.00} ₽";
    public string AvgRatingText => _model.AvgRating == null ? "Нет оценок" : $"★ {_model.AvgRating:0.0}";

    [ObservableProperty]
    private ImageSource? _image;
    public async Task LoadImageAsync() => Image = await _imageService.GetImageAsync(_model.ImageUrl);


    [RelayCommand]
    public Task OpenProductAsync() =>
        _navService.NavigateToAsync<ProductDetailsViewModel>(_model.Id);

}
