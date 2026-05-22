using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Enums;
using DeliverySystem.Infrastructure.Database;
using DeliverySystem.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DeliverySystem.Tests;

public sealed class RepositoryIntegrationTests : IAsyncLifetime
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    private DeliveryDbContext _db = null!;

    public async Task InitializeAsync()
    {
        await _connection.OpenAsync();

        var options = new DbContextOptionsBuilder<DeliveryDbContext>()
            .UseSqlite(_connection)
            .Options;

        _db = new DeliveryDbContext(options);
        await _db.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _db.DisposeAsync();
        await _connection.DisposeAsync();
    }

    [Fact]
    public async Task RestauranteRepository_DeveBuscarPorNome()
    {
        var repo = new RestauranteRepository(_db);
        await repo.AddAsync(new Restaurante("Pizza Prime", "Rua 10", "1111-1111", "Pizza"));
        await repo.AddAsync(new Restaurante("Taco House", "Rua 20", "2222-2222", "Mexicana"));
        await _db.SaveChangesAsync();

        var encontrados = await repo.SearchByNomeAsync("Pizza");

        Assert.Single(encontrados);
        Assert.Equal("Pizza Prime", encontrados[0].Nome);
    }

    [Fact]
    public async Task CupomRepository_DeveRecuperarCupomPorCodigo()
    {
        var repo = new CupomRepository(_db);
        await repo.AddAsync(new Cupom("PROMO15", CupomTipo.Percentual, 15m, true));
        await _db.SaveChangesAsync();

        var cupom = await repo.GetByCodigoAsync("promo15");

        Assert.NotNull(cupom);
        Assert.Equal(15m, cupom!.Valor);
    }
}
