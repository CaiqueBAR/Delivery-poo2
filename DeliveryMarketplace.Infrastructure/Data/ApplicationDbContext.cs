using Microsoft.EntityFrameworkCore;

namespace DeliveryMarketplace.Infrastructure.Data;

/// <summary>
/// Contexto do Entity Framework Core para o banco SQLite (delivery.db).
/// Os DbSet das entidades serão adicionados na Fase 2, depois que o diagrama UML for aprovado.
/// As configurações Fluent API de cada entidade ficarão em Configurations/ e são aplicadas automaticamente.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
