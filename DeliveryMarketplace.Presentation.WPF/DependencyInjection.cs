using DeliveryMarketplace.Presentation.WPF.Navigation;
using DeliveryMarketplace.Presentation.WPF.ViewModels;
using DeliveryMarketplace.Presentation.WPF.Views;
using Microsoft.Extensions.DependencyInjection;

namespace DeliveryMarketplace.Presentation.WPF;

/// <summary>
/// Registro dos componentes da camada de apresentação: navegação, janelas e ViewModels.
/// Para adicionar uma nova tela: registre o ViewModel aqui e mapeie-o para a View
/// em Resources/ViewTemplates.xaml.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        // Navegação
        services.AddSingleton<INavigationService, NavigationService>();

        // Janela principal
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<MainWindow>();

        // Páginas (nova instância a cada navegação)
        services.AddTransient<HomeViewModel>();
        services.AddTransient<SystemStatusViewModel>();

        return services;
    }
}
