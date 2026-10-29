using CactiShop.Desktop.Converters;
using CactiShop.Desktop.Services;
using CactiShop.Desktop.Services.Api;
using CactiShop.Desktop.Services.Fake;
using CactiShop.Desktop.Services.Interfaces;
using CactiShop.Desktop.ViewModels;
using CactiShop.Desktop.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using System.Windows.Threading;

namespace CactiShop.Desktop;

public partial class App : Application
{
    private readonly IHost _host;

    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .UseContentRoot(AppContext.BaseDirectory)
            .ConfigureServices((context, services) => ConfigureServices(context.Configuration, services))
            .Build();

        DispatcherUnhandledException += OnUnhandledException;
    }

    private static void ConfigureServices(IConfiguration config, IServiceCollection services)
    {
        services.AddSingleton<ISessionService, SessionService>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IDialogService, DialogService>();

        if (config.GetValue<bool>("Api:UseFake"))
        {
            services.AddSingleton<ICatalogService, FakeCatalogService>();
        }
        else
        {
            var baseUrl = config["Api:BaseUrl"] ?? "https://localhost:5001/";
            services.AddHttpClient<ApiClient>(http =>
            {
                http.BaseAddress = new Uri(baseUrl);
                http.Timeout = TimeSpan.FromSeconds(15);
            });
            services.AddTransient<ICatalogService, CatalogService>();
        }

        services.AddSingleton<MainViewModel>();
        services.AddTransient<CatalogViewModel>();
        services.AddTransient<ProductDetailsViewModel>();

        services.AddSingleton<MainWindow>();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        await _host.StartAsync();

        var config = _host.Services.GetRequiredService<IConfiguration>();
        ((ImageUrlConverter)Resources["ImageUrl"]).BaseAddress = config.GetValue<bool>("Api:UseFake")
            ? new Uri(AppContext.BaseDirectory)
            : new Uri(config["Api:BaseUrl"] ?? "https://localhost:5001/");

        _host.Services.GetRequiredService<MainWindow>().Show();

        await _host.Services.GetRequiredService<INavigationService>().NavigateToAsync<CatalogViewModel>();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();
        base.OnExit(e);
    }

    private void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        _host.Services.GetRequiredService<IDialogService>().ShowError("Что-то пошло не так: " + e.Exception.Message);
        e.Handled = true;
    }
}