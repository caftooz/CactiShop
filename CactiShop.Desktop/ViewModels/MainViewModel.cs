using CactiShop.Contracts.Auth;
using CactiShop.Desktop.Models;
using CactiShop.Desktop.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CactiShop.Desktop.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ISessionService _sessionService;
    private readonly IDialogService _dialogService;

    public MainViewModel(INavigationService navigationService, ISessionService sessionService, IDialogService dialogService)
    {
        NavigationService = navigationService;
        _sessionService = sessionService;
        _dialogService = dialogService;

        _sessionService.CurrentUserChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(IsLoggedIn));
            OnPropertyChanged(nameof(IsGuest));
            OnPropertyChanged(nameof(IsAdmin));
            OnPropertyChanged(nameof(UserTitle));
        };
    }

    public INavigationService NavigationService { get; }        

    public bool IsLoggedIn => _sessionService.IsLoggedIn;
    public bool IsGuest => !_sessionService.IsLoggedIn;
    public bool IsAdmin => _sessionService.IsAdmin;
    public string UserTitle => _sessionService.CurrentUser?.Login ?? "Гость";


    [RelayCommand]
    private Task OpenCatalogAsync() => NavigationService.NavigateToAsync<CatalogViewModel>();

    [RelayCommand]
    private void OpenCart() => _dialogService.ShowInfo("Корзина");
    [RelayCommand]
    private void OpenFavorites() => _dialogService.ShowInfo("Избранное");

    [RelayCommand]
    private void OpenProfile() => _dialogService.ShowInfo("Мой профиль");

    [RelayCommand]
    private void OpenAdmin() => _dialogService.ShowInfo("Админ панель");

    [RelayCommand]
    private void SignIn()
    {
        var asAdmin = _dialogService.Confirm("Войти как админ? Нет - как обычный пользователь.");
        _sessionService.SignIn(new AppUser(asAdmin ? "admin1" : "user1", asAdmin ? UserRole.Admin : UserRole.User), "demo-token");
    }

    [RelayCommand]
    private void SignOut() => _sessionService.SignOut();
}
