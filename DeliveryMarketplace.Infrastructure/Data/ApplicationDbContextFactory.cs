using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DeliveryMarketplace.Infrastructure.Data;

/// <summary>
/// Fábrica usada somente em tempo de design pelas ferramentas "dotnet ef"
/// (geração de migrations na Fase 2). Não é usada pela aplicação em execução.
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(SqliteConnectionStringResolver.DefaultConnectionString)
            .Options;

        return new ApplicationDbContext(options);
    }
}
