using System.Windows;
using CactiShop.Desktop.ViewModels;

namespace CactiShop.Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
