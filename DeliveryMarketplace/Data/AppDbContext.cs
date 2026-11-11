using DeliveryMarketplace.Models;
using Microsoft.EntityFrameworkCore;

namespace DeliveryMarketplace.Data;

/// <summary>
/// Contexto do Entity Framework. Cada DbSet vira uma tabela no arquivo delivery.db.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>O banco fica na mesma pasta do executável.</summary>
    public static string CaminhoBanco => Path.Combine(AppContext.BaseDirectory, "delivery.db");

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Restaurante> Restaurantes => Set<Restaurante>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<ItemPedido> ItensPedido => Set<ItemPedido>();
    public DbSet<Pagamento> Pagamentos => Set<Pagamento>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={CaminhoBanco}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // E-mail único para o login
        modelBuilder.Entity<Usuario>().HasIndex(u => u.Email).IsUnique();

        // Enums gravados como texto (fica legível ao abrir o banco)
        modelBuilder.Entity<Usuario>().Property(u => u.Perfil).HasConversion<string>();
        modelBuilder.Entity<Pedido>().Property(p => p.Status).HasConversion<string>();
        modelBuilder.Entity<Pagamento>().Property(p => p.Metodo).HasConversion<string>();
        modelBuilder.Entity<Pagamento>().Property(p => p.Status).HasConversion<string>();

        // Relacionamentos de Pedido com usuários (cliente e entregador são da mesma tabela)
        modelBuilder.Entity<Pedido>()
            .HasOne(p => p.Cliente).WithMany().HasForeignKey(p => p.ClienteId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Pedido>()
            .HasOne(p => p.Entregador).WithMany().HasForeignKey(p => p.EntregadorId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Pedido>()
            .HasOne(p => p.Pagamento).WithOne().HasForeignKey<Pagamento>(p => p.PedidoId);

        modelBuilder.Entity<Restaurante>()
            .HasOne(r => r.Gerente).WithMany().HasForeignKey(r => r.GerenteId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Produto>()
            .HasOne(p => p.Categoria).WithMany().HasForeignKey(p => p.CategoriaId).OnDelete(DeleteBehavior.Restrict);
    }
}
