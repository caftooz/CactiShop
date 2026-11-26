using System.Windows;
using System.Windows.Controls;
using CactiShop.Desktop.ViewModels;

namespace CactiShop.Desktop.Views;

public partial class CatalogView : UserControl
{
    public CatalogView()
    {
        InitializeComponent();
    }
    private async void ProductCard_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: ProductSummaryViewModel vm })
        {
            await vm.LoadDataAsync();
        }
    }

    private async void ProductCard_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is ProductSummaryViewModel vm)
        {
            await vm.LoadDataAsync();
        }
    }
}
