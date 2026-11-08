using System.Net.Http;
using CactiShop.Desktop.Services.Api;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CactiShop.Desktop.ViewModels;

/// <summary>
///     Базовый класс для всех страниц: загрузка, ошибка и единая обработка исключений.
/// </summary>
public abstract partial class PageViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string? _errorMessage;

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    /// <summary>
    ///     Вызывается навигацией сразу после открытия страницы.
    /// </summary>
    public virtual Task OnNavigatedToAsync(object? parameter) => Task.CompletedTask;

    /// <summary>
    ///     Выполнить обращение к сервису: показать загрузку, поймать ошибку, положить её текст в ErrorMessage.
    /// </summary>
    protected async Task RunAsync(Func<Task> action)
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = null;
        try
        {
            await action();
        }
        catch (ApiException ex)
        {
            ErrorMessage = ex.Message;
        }
        catch (HttpRequestException)
        {
            ErrorMessage = "Нет связи с сервером. Проверьте, что API запущено.";
        }
        catch (TaskCanceledException)
        {
            ErrorMessage = "Сервер не ответил вовремя.";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
