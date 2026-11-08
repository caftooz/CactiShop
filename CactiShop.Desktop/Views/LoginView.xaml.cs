using System.Windows;
using System.Windows.Controls;
using CactiShop.Desktop.ViewModels;

namespace CactiShop.Desktop.Views;
/// <summary>
/// Логика взаимодействия для LoginView.xaml
/// </summary>
public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();
    }
    private void OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoginViewModel vm)
            vm.Password = ((PasswordBox)sender).Password;
    }
}
