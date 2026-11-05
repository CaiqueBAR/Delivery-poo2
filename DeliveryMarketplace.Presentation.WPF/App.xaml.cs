using System.Windows;
using System.Windows.Threading;
using DeliveryMarketplace.Application;
using DeliveryMarketplace.Infrastructure;
using DeliveryMarketplace.Presentation.WPF.ViewModels;
using DeliveryMarketplace.Presentation.WPF.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DeliveryMarketplace.Presentation.WPF;

/// <summary>
/// Ponto de entrada do WPF. Monta o Host genérico (DI + configuração + logging),
/// registra as camadas e abre a janela principal.
/// </summary>
public partial class App : System.Windows.Application
{
    private IHost? _host;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        DispatcherUnhandledException += OnDispatcherUnhandledException;

        try
        {
            var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
            {
                Args = e.Args,
                // Lê o appsettings.json da pasta do executável, independentemente de onde foi iniciado.
                ContentRootPath = AppContext.BaseDirectory
            });

            builder.Services
                .AddApplication()
                .AddInfrastructure(builder.Configuration)
                .AddPresentation();

            _host = builder.Build();
            await _host.StartAsync();

            var window = _host.Services.GetRequiredService<MainWindow>();
            MainWindow = window;
            window.Show();

            await _host.Services.GetRequiredService<MainWindowViewModel>().InitializeAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Não foi possível iniciar a aplicação.\n\n{ex.Message}",
                "DeliveryMarketplace", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(-1);
        }
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host is not null)
        {
            await _host.StopAsync(TimeSpan.FromSeconds(5));
            _host.Dispose();
        }

        base.OnExit(e);
    }

    /// <summary>Tratamento global de exceções não capturadas na thread de interface.</summary>
    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        _host?.Services.GetService<ILogger<App>>()?.LogError(e.Exception, "Erro não tratado na interface");

        MessageBox.Show($"Ocorreu um erro inesperado.\n\n{e.Exception.Message}",
            "DeliveryMarketplace", MessageBoxButton.OK, MessageBoxImage.Error);

        e.Handled = true;
    }
}
