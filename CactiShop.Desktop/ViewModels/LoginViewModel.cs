using CactiShop.Desktop.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CactiShop.Desktop.ViewModels;

public partial class LoginViewModel(
    IAuthService _authService,
    INavigationService _navigationService) : PageViewModel
{
    [ObservableProperty]
    private string _login = "";

    public string Password { get; set; } = "";

    [RelayCommand]
    private Task SignInAsync()
    {
        if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrEmpty(Password))
        {
            ErrorMessage = "Введите логин и пароль.";
            return Task.CompletedTask;
        }

        return RunAsync(async () =>
        {
            await _authService.LoginAsync(Login.Trim(), Password);
            await _navigationService.NavigateToAsync<CatalogViewModel>();
        });
    }

    [RelayCommand]
    private Task OpenRegisterAsync() => _navigationService.NavigateToAsync<RegisterViewModel>();

}
