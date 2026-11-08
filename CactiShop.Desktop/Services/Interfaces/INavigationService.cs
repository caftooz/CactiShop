using CactiShop.Desktop.ViewModels;

namespace CactiShop.Desktop.Services.Interfaces;

public interface INavigationService
{
    PageViewModel? CurrentPage { get; }
    bool CanGoBack { get; }
    Task NavigateToAsync<TViewModel>(object? parameter = null, bool clearHistory = false) where TViewModel : PageViewModel;
    void GoBack();
}
