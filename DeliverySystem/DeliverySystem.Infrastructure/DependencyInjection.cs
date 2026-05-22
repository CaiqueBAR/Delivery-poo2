using DeliverySystem.Application.Interfaces;
using DeliverySystem.Infrastructure.Database;
using DeliverySystem.Infrastructure.Repositories;
using DeliverySystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DeliverySystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string 'Default' não encontrada.");
        var jwtSection = config.GetSection(JwtOptions.SectionName);
        var jwtOptions = new JwtOptions
        {
            Issuer = jwtSection["Issuer"] ?? "DeliverySystem",
            Audience = jwtSection["Audience"] ?? "DeliverySystem.Clients",
            Key = jwtSection["Key"] ?? "super-secret-dev-key-with-32-characters",
            ExpirationMinutes = int.TryParse(jwtSection["ExpirationMinutes"], out var exp) ? exp : 120
        };

        DatabaseConnectionSingleton.Instance.Configure(connectionString);

        services.AddDbContext<DeliveryDbContext>(opt => opt.UseSqlServer(connectionString));
        services.AddSingleton<IOptions<JwtOptions>>(Options.Create(jwtOptions));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IRestauranteRepository, RestauranteRepository>();
        services.AddScoped<IProdutoRepository, ProdutoRepository>();
        services.AddScoped<IPedidoRepository, PedidoRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<ICupomRepository, CupomRepository>();
        services.AddScoped<IClienteRepository, ClienteRepository>();

        services.AddSingleton<INotificationService, InMemoryNotificationService>();
        services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        return services;
    }
}
