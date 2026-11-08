using CactiShop.Desktop.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CactiShop.Desktop.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ISessionService _sessionService;
    private readonly IDialogService _dialogService;
    private readonly IAuthService _authService;

    public MainViewModel(INavigationService navigationService, ISessionService sessionService, IDialogService dialogService, IAuthService authService)
    {
        NavigationService = navigationService;
        _sessionService = sessionService;
        _dialogService = dialogService;
        _authService = authService;

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
    private Task OpenCatalogAsync() => NavigationService.NavigateToAsync<CatalogViewModel>(clearHistory: true);

    [RelayCommand]
    private void OpenCart() => _dialogService.ShowInfo("Корзина");
    [RelayCommand]
    private void OpenFavorites() => _dialogService.ShowInfo("Избранное");

    [RelayCommand]
    private void OpenProfile() => _dialogService.ShowInfo("Мой профиль");

    [RelayCommand]
    private void OpenAdmin() => _dialogService.ShowInfo("Админ панель");

    [RelayCommand]
    private Task SignInAsync() => NavigationService.NavigateToAsync<LoginViewModel>();

    [RelayCommand]
    private void SignOut() => _authService.Logout();
}
