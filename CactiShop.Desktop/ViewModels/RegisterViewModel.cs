using CactiShop.Desktop.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CactiShop.Desktop.ViewModels;

public partial class RegisterViewModel(
    IAuthService _authService,
    INavigationService _navigationService) : PageViewModel
{
    [ObservableProperty]
    private string _login = "";

    public string Password { get; set; } = "";
    public string ConfirmPassword { get; set; } = "";

    [RelayCommand]
    private Task RegisterAsync()
    {
        if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(ConfirmPassword))
        {
            ErrorMessage = "Введите логин и пароль.";
            return Task.CompletedTask;
        }

        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Пароли не совпадают.";
            return Task.CompletedTask;
        }

        return RunAsync(async () =>
        {
            await _authService.RegisterAsync(Login.Trim(), Password, ConfirmPassword);
            await _authService.LoginAsync(Login.Trim(), Password);
            await _navigationService.NavigateToAsync<CatalogViewModel>();
        });
    }

    [RelayCommand]
    private Task OpenLoginAsync() => _navigationService.NavigateToAsync<LoginViewModel>();
}
