using DeliveryMarketplace.Application.Interfaces;
using DeliveryMarketplace.Infrastructure.Data;
using DeliveryMarketplace.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DeliveryMarketplace.Infrastructure;

/// <summary>
/// Registro dos serviços da camada Infrastructure (EF Core + SQLite, repositórios e serviços técnicos).
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string? baseDirectory = null)
    {
        var connectionString = SqliteConnectionStringResolver.Resolve(
            configuration.GetConnectionString("DefaultConnection"),
            baseDirectory ?? AppContext.BaseDirectory);

        // Numa aplicação desktop não existe "escopo por requisição".
        // A fábrica cria um DbContext curto para cada operação, evitando um contexto de vida longa.
        services.AddDbContextFactory<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddTransient<IDatabaseStatusService, DatabaseStatusService>();

        return services;
    }
}
