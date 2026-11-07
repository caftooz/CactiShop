using CactiShop.Desktop.Models;
using CactiShop.Desktop.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CactiShop.Desktop.ViewModels;

public partial class ProductSummaryViewModel : ObservableObject
{
    private ProductSummary _model;
    private INavigationService _navService;

    public ProductSummaryViewModel(ProductSummary model, INavigationService navService, IImageService imageService)
    {
        _model = model;
        _navService = navService;

        Image = imageService.GetImage(model.ImageUrl);
    }

    public string Name          => _model.Name;
    public string CategoryName  => _model.CategoryName;
    public string PriceText     => $"{_model.Price:N0} ₽";
    public string AvgRatingText => _model.AvgRating == null ? "Нет оценок" : $"★ {_model.AvgRating:0.0}";

    public BitmapImage Image { get; }


    [RelayCommand]
    public Task OpenProductAsync() =>
        _navService.NavigateToAsync<ProductDetailsViewModel>(_model.Id);

}
