using DeliveryMarketplace.Application;
using DeliveryMarketplace.Application.Interfaces;
using DeliveryMarketplace.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DeliveryMarketplace.Tests.IntegrationTests;

/// <summary>
/// Monta o container de DI como a aplicação faz e verifica o provedor SQLite real.
/// </summary>
public class InfrastructureRegistrationTests : IDisposable
{
    private readonly string _baseDir = Path.Combine(Path.GetTempPath(), "dm-tests-" + Guid.NewGuid().ToString("N"));

    private ServiceProvider BuildProvider()
    {
        Directory.CreateDirectory(_baseDir);

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Data Source=delivery.db"
            })
            .Build();

        return new ServiceCollection()
            .AddApplication()
            .AddInfrastructure(configuration, _baseDir)
            .BuildServiceProvider(validateScopes: true);
    }

    [Fact]
    public async Task Provedor_sqlite_funciona_e_aponta_para_delivery_db()
    {
        using var provider = BuildProvider();
        var service = provider.GetRequiredService<IDatabaseStatusService>();

        var status = await service.GetStatusAsync();

        Assert.True(status.ProvedorFuncional, status.Erro);
        Assert.StartsWith("3.", status.VersaoSqlite);
        Assert.Equal("Microsoft.EntityFrameworkCore.Sqlite", status.Provedor);
        Assert.Equal(Path.Combine(_baseDir, "delivery.db"), status.CaminhoArquivo);
    }

    [Fact]
    public async Task Verificacao_nao_cria_o_arquivo_do_banco_na_fase_1()
    {
        using var provider = BuildProvider();
        var service = provider.GetRequiredService<IDatabaseStatusService>();

        var status = await service.GetStatusAsync();

        Assert.False(status.ArquivoExiste);
        Assert.False(File.Exists(Path.Combine(_baseDir, "delivery.db")));
    }

    public void Dispose()
    {
        if (Directory.Exists(_baseDir))
        {
            Directory.Delete(_baseDir, recursive: true);
        }
    }
}
