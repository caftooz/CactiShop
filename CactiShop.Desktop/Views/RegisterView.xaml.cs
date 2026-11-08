using System.Windows;
using System.Windows.Controls;
using CactiShop.Desktop.ViewModels;

namespace CactiShop.Desktop.Views;
/// <summary>
/// Логика взаимодействия для RegisterView.xaml
/// </summary>
public partial class RegisterView : UserControl
{
    public RegisterView()
    {
        InitializeComponent();
    }
    private void OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is RegisterViewModel vm)
            vm.Password = ((PasswordBox)sender).Password;
    }
    private void OnConfirmPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is RegisterViewModel vm)
            vm.ConfirmPassword = ((PasswordBox)sender).Password;
    }
}
