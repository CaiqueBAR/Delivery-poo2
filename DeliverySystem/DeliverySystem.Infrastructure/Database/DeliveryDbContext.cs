using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DeliverySystem.Infrastructure.Database;

public sealed class DeliveryDbContext : DbContext
{
    public DeliveryDbContext(DbContextOptions<DeliveryDbContext> options) : base(options)
    {
    }

    public DbSet<Restaurante> Restaurantes => Set<Restaurante>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Prato> Pratos => Set<Prato>();
    public DbSet<Bebida> Bebidas => Set<Bebida>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<ItemDePedido> ItensPedido => Set<ItemDePedido>();
    public DbSet<Cupom> Cupons => Set<Cupom>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Entregador> Entregadores => Set<Entregador>();
    public DbSet<AvaliacaoProduto> AvaliacoesProduto => Set<AvaliacaoProduto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Restaurante>(b =>
        {
            b.ToTable("Restaurantes");
            b.HasKey(x => x.Id);
            b.Property(x => x.Nome).HasMaxLength(200).IsRequired();
            b.Property(x => x.Endereco).HasMaxLength(300).IsRequired();
            b.Property(x => x.Telefone).HasMaxLength(50).IsRequired();
            b.Property(x => x.Categoria).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Produto>(b =>
        {
            b.ToTable("Produtos");
            b.HasKey(x => x.Id);
            b.Property(x => x.RestauranteId).IsRequired();
            b.Property(x => x.Nome).HasMaxLength(200).IsRequired();
            b.Property(x => x.Descricao).HasMaxLength(1000).IsRequired();
            b.Property(x => x.Preco).HasColumnType("decimal(18,2)").IsRequired();
            b.Property(x => x.TempoPreparoMin).IsRequired();
            b.Property(x => x.ImagemPath).HasMaxLength(500);
        });

        modelBuilder.Entity<Prato>(b =>
        {
            b.ToTable("Pratos");
            b.Property(x => x.TipoCozinha).HasMaxLength(100).IsRequired();
            b.Property(x => x.Tamanho).HasMaxLength(50).IsRequired();
            b.Property(x => x.Ingredientes).HasMaxLength(1000).IsRequired();
        });

        modelBuilder.Entity<Bebida>(b =>
        {
            b.ToTable("Bebidas");
            b.Property(x => x.VolumeMl).IsRequired();
            b.Property(x => x.Gelada).IsRequired();
            b.Property(x => x.Tipo).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Pedido>(b =>
        {
            b.ToTable("Pedidos");
            b.HasKey(x => x.Id);
            b.Property(x => x.DataPedido).IsRequired();
            b.Property(x => x.Status).IsRequired();
            b.Property(x => x.ValorTotal).HasColumnType("decimal(18,2)").IsRequired();
            b.Property(x => x.ClienteNome).HasMaxLength(200).IsRequired();
            b.Property(x => x.EnderecoEntrega).HasMaxLength(300).IsRequired();
            b.HasMany(x => x.Itens).WithOne().OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ItemDePedido>(b =>
        {
            b.ToTable("ItensPedido");
            b.HasKey(x => x.Id);
            b.Property(x => x.Quantidade).IsRequired();
            b.Property(x => x.Subtotal).HasColumnType("decimal(18,2)").IsRequired();
            b.HasOne(x => x.Produto).WithMany().OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Cupom>(b =>
        {
            b.ToTable("Cupons");
            b.HasKey(x => x.Id);
            b.Property(x => x.Codigo).HasMaxLength(50).IsRequired();
            b.HasIndex(x => x.Codigo).IsUnique();
            b.Property(x => x.Tipo).IsRequired();
            b.Property(x => x.Valor).HasColumnType("decimal(18,2)").IsRequired();
            b.Property(x => x.Ativo).IsRequired();
        });

        modelBuilder.Entity<Usuario>(b =>
        {
            b.ToTable("Usuarios");
            b.HasKey(x => x.Id);
            b.Property(x => x.Email).HasMaxLength(200).IsRequired();
            b.HasIndex(x => x.Email).IsUnique();
            b.Property(x => x.PasswordHash).HasMaxLength(200).IsRequired();
            b.Property(x => x.Role).IsRequired();
            b.Property(x => x.Ativo).IsRequired();
        });

        modelBuilder.Entity<Cliente>(b =>
        {
            b.ToTable("Clientes");
            b.HasKey(x => x.Id);
            b.Property(x => x.UsuarioId).IsRequired();
            b.Property(x => x.Nome).HasMaxLength(200).IsRequired();
            b.Property(x => x.Telefone).HasMaxLength(50).IsRequired();
            b.Property(x => x.EnderecoPadrao).HasMaxLength(300).IsRequired();
        });

        modelBuilder.Entity<Entregador>(b =>
        {
            b.ToTable("Entregadores");
            b.HasKey(x => x.Id);
            b.Property(x => x.Nome).HasMaxLength(200).IsRequired();
            b.Property(x => x.Telefone).HasMaxLength(50).IsRequired();
            b.Property(x => x.Veiculo).HasMaxLength(100).IsRequired();
            b.Property(x => x.Ativo).IsRequired();
        });

        modelBuilder.Entity<AvaliacaoProduto>(b =>
        {
            b.ToTable("AvaliacoesProduto");
            b.HasKey(x => x.Id);
            b.Property(x => x.ProdutoId).IsRequired();
            b.Property(x => x.ClienteId).IsRequired();
            b.Property(x => x.Nota).IsRequired();
            b.Property(x => x.Comentario).HasMaxLength(1000).IsRequired();
            b.Property(x => x.DataCriacao).IsRequired();
        });

        modelBuilder.Entity<Restaurante>().HasData(
            new { Id = 1, Nome = "Burger House", Endereco = "Rua A, 123", Telefone = "(11) 99999-0000", Categoria = "Hamburguer" },
            new { Id = 2, Nome = "Sushi Now", Endereco = "Av. B, 456", Telefone = "(11) 98888-0000", Categoria = "Japonesa" }
        );

        modelBuilder.Entity<Cupom>().HasData(
            new { Id = 1, Codigo = "CUPOM10", Tipo = CupomTipo.Percentual, Valor = 10m, Ativo = true },
            new { Id = 2, Codigo = "DESCONTO20", Tipo = CupomTipo.Percentual, Valor = 20m, Ativo = true },
            new { Id = 3, Codigo = "FRETEGRATIS", Tipo = CupomTipo.FreteGratis, Valor = 0m, Ativo = true }
        );
    }
}
