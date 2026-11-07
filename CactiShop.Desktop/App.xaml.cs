using CactiShop.Desktop.Converters;
using CactiShop.Desktop.Factories;
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
        // Global services
        services.AddSingleton<ISessionService, SessionService>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IDialogService, DialogService>();

        // Options
        var apiOptions = config.GetSection(ApiOptions.SectionName).Get<ApiOptions>()
                 ?? throw new InvalidOperationException("Нет секции Api в appsettings.json");

        services.AddOptions<ApiOptions>()
            .Bind(config.GetSection(ApiOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // API services
        if (apiOptions.UseFake)
        {
            services.AddSingleton<ICatalogService, FakeCatalogService>();
            services.AddSingleton<IImageService>(new ImageService(new Uri(AppContext.BaseDirectory)));
        }
        else
        {
            var baseUrl = apiOptions.BaseUrl ?? "https://localhost:5001/";
            services.AddHttpClient<ApiClient>(http =>
            {
                http.BaseAddress = new Uri(baseUrl);
                http.Timeout = TimeSpan.FromSeconds(15);
            });
            services.AddTransient<ICatalogService, CatalogService>();
            services.AddSingleton<IImageService>(new ImageService(new Uri(baseUrl)));
        }

        // ViewModels
        services.AddSingleton<MainViewModel>();
        services.AddTransient<CatalogViewModel>();
        services.AddTransient<ProductDetailsViewModel>();

        // Factories
        services.AddSingleton<ProductSummaryViewModelFactory>();

        // Windows
        services.AddSingleton<MainWindow>();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        await _host.StartAsync();

        _host.Services.GetRequiredService<MainWindow>().Show();

        await _host.Services.GetRequiredService<INavigationService>().NavigateToAsync<CatalogViewModel>();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _host.StopAsync(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();
        _host.Dispose();
        base.OnExit(e);
    }

    private void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        _host.Services.GetRequiredService<IDialogService>().ShowError("Что-то пошло не так: " + e.Exception.Message);
        e.Handled = true;
    }
}