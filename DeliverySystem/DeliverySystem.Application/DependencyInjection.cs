using DeliverySystem.Application.Services;
using DeliverySystem.Application.Services.Observers;
using Microsoft.Extensions.DependencyInjection;

namespace DeliverySystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<RestauranteAppService>();
        services.AddScoped<ProdutoAppService>();
        services.AddScoped<PedidoAppService>();
        services.AddScoped<RelatorioAppService>();
        services.AddScoped<AuthAppService>();
        services.AddScoped<ClienteAppService>();
        services.AddScoped<ClientePedidoObserver>();

        return services;
    }
}
