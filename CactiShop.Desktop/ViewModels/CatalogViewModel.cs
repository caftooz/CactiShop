using System.Collections.ObjectModel;
using System.Globalization;
using CactiShop.Contracts.Catalog;
using CactiShop.Desktop.Factories;
using CactiShop.Desktop.Models;
using CactiShop.Desktop.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CactiShop.Desktop.ViewModels;

public partial class CatalogViewModel(
    ICatalogService _catalogService,
    ProductSummaryViewModelFactory _factory
    ) : PageViewModel
{
    private static readonly Category AllCategories = new(0, "Все категории");
    public ObservableCollection<ProductSummaryViewModel> Products { get; } = new();
    public ObservableCollection<Category> Categories { get; } = [];

    [ObservableProperty]
    private string _searchText = "";


    [ObservableProperty]
    private Category? _selectedCategory;

    [ObservableProperty]
    private string _minPriceText = "";

    [ObservableProperty]
    private string _maxPriceText = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    private bool _isLoaded;
    public bool IsEmpty => IsLoaded && Products.Count == 0;

    public override Task OnNavigatedToAsync(object? parameter) => RunAsync(async () =>
    {
        var categories = await _catalogService.GetCategoriesAsync();
        Categories.Clear();
        Categories.Add(AllCategories);
        foreach (var c in categories) Categories.Add(c);
        SelectedCategory = AllCategories;

        await LoadProductsAsync();
    });

    [RelayCommand]
    private Task SearchAsync() => RunAsync(LoadProductsAsync);

    [RelayCommand]
    private Task ResetFiltersAsync()
    {
        SearchText = "";
        MinPriceText = "";
        MaxPriceText = "";
        SelectedCategory = AllCategories;
        return RunAsync(LoadProductsAsync);
    }

    private async Task LoadProductsAsync()
    {
        if (!TryReadPrice(MinPriceText, out var min) || !TryReadPrice(MaxPriceText, out var max))
        {
            ErrorMessage = "Цена должна быть числом, например 150 или 99,90.";
            return;
        }

        var query = new ProductQuery(
            Query: SearchText,
            CategoryId: SelectedCategory is { Id: > 0 } c ? c.Id : null,
            MinPrice: min,
            MaxPrice: max);

        var items = await _catalogService.GetCatalogAsync(query);

        Products.Clear();
        foreach (var p in items)
            Products.Add(_factory.Create(p));

        IsLoaded = true;
        OnPropertyChanged(nameof(IsEmpty));
    }

    private static bool TryReadPrice(string text, out decimal? value)
    {
        value = null;
        if (string.IsNullOrWhiteSpace(text)) return true;

        var normalized = text.Trim().Replace('.', ',');
        if (decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.GetCultureInfo("ru-RU"), out var d) && d >= 0)
        {
            value = d;
            return true;
        }
        return false;
    }
}
