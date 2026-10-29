using CactiShop.Desktop.ViewModels;
using System.Windows;

namespace CactiShop.Desktop.Views;
public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}