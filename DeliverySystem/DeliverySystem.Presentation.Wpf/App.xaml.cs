using System.Windows;
using DeliverySystem.Application;
using DeliverySystem.Infrastructure;
using DeliverySystem.Presentation.Wpf.Services;
using DeliverySystem.Presentation.Wpf.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DeliverySystem.Presentation.Wpf;

public partial class App : System.Windows.Application
{
    private readonly IHost _host;

    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(config =>
            {
                config.SetBasePath(AppContext.BaseDirectory);
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.AddApplication();
                services.AddInfrastructure(context.Configuration);
                services.AddSingleton<CartState>();
                services.AddSingleton<ScopedRunner>();
                services.AddSingleton<IProductDetailsDialogService, ProductDetailsDialogService>();

                services.AddTransient<HomePageViewModel>();
                services.AddTransient<RestaurantsPageViewModel>();
                services.AddTransient<ProductsPageViewModel>();
                services.AddTransient<CartPageViewModel>();
                services.AddTransient<DashboardPageViewModel>();
                services.AddTransient<MainViewModel>();
                services.AddTransient<MainWindow>();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await _host.StartAsync();
        var window = _host.Services.GetRequiredService<MainWindow>();
        window.Show();
        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();
        base.OnExit(e);
    }
}
