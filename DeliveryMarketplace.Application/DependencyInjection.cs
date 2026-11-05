using Microsoft.Extensions.DependencyInjection;

namespace DeliveryMarketplace.Application;

/// <summary>
/// Ponto único de registro dos serviços da camada Application.
/// Os serviços de negócio (pedidos, pagamentos, entregas...) serão registrados aqui
/// nas próximas fases.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}
