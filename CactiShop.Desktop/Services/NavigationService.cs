using CactiShop.Desktop.Services.Interfaces;
using CactiShop.Desktop.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace CactiShop.Desktop.Services;

public partial class NavigationService : ObservableObject, INavigationService
{
    private readonly IServiceProvider _services;
    private readonly Stack<PageViewModel> _history = new();
    public bool CanGoBack => _history.Count > 0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanGoBack))]
    private PageViewModel? _currentPage;

    public NavigationService(IServiceProvider services)
    {
        _services = services;
    }

    public async Task NavigateToAsync<TViewModel>(object? parameter = null) where TViewModel : PageViewModel
    {
        var page = _services.GetRequiredService<TViewModel>();

        if (CurrentPage is not null)
            _history.Push(CurrentPage);

        CurrentPage = page;

        await page.OnNavigatedToAsync(parameter);
    }

    public void GoBack()
    {
        if (CanGoBack)
            CurrentPage = _history.Pop();
    }

}
