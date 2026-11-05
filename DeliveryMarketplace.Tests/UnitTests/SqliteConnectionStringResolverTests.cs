using DeliveryMarketplace.Infrastructure.Data;
using Microsoft.Data.Sqlite;

namespace DeliveryMarketplace.Tests.UnitTests;

/// <summary>Garante que o caminho do delivery.db é resolvido corretamente.</summary>
public class SqliteConnectionStringResolverTests
{
    private static readonly string BaseDir = Path.Combine(Path.GetTempPath(), "dm-tests");

    [Fact]
    public void Caminho_relativo_e_resolvido_na_pasta_base()
    {
        var resultado = SqliteConnectionStringResolver.Resolve("Data Source=delivery.db", BaseDir);

        var dataSource = new SqliteConnectionStringBuilder(resultado).DataSource;
        Assert.Equal(Path.GetFullPath(Path.Combine(BaseDir, "delivery.db")), dataSource);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Sem_connection_string_usa_delivery_db(string? connectionString)
    {
        var resultado = SqliteConnectionStringResolver.Resolve(connectionString, BaseDir);

        Assert.EndsWith("delivery.db", new SqliteConnectionStringBuilder(resultado).DataSource);
    }

    [Fact]
    public void Caminho_absoluto_nao_e_alterado()
    {
        var absoluto = Path.Combine(Path.GetTempPath(), "outro", "banco.db");

        var resultado = SqliteConnectionStringResolver.Resolve($"Data Source={absoluto}", BaseDir);

        Assert.Equal(absoluto, new SqliteConnectionStringBuilder(resultado).DataSource);
    }

    [Fact]
    public void Banco_em_memoria_nao_e_alterado()
    {
        var resultado = SqliteConnectionStringResolver.Resolve("Data Source=:memory:", BaseDir);

        Assert.Equal(":memory:", new SqliteConnectionStringBuilder(resultado).DataSource);
    }
}
