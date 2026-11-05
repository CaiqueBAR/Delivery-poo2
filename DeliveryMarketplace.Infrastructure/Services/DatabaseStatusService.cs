using DeliveryMarketplace.Application.DTOs;
using DeliveryMarketplace.Application.Interfaces;
using DeliveryMarketplace.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DeliveryMarketplace.Infrastructure.Services;

/// <summary>
/// Verifica se o provedor SQLite está funcional e onde o delivery.db será criado.
/// Na Fase 1 ele NÃO cria o arquivo do banco: o teste de conexão é feito em memória.
/// </summary>
public sealed class DatabaseStatusService : IDatabaseStatusService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public DatabaseStatusService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<DatabaseStatusDto> GetStatusAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var connectionString = context.Database.GetConnectionString() ?? string.Empty;
        var caminho = new SqliteConnectionStringBuilder(connectionString).DataSource;
        var provedor = context.Database.ProviderName ?? "desconhecido";

        try
        {
            await using var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync(cancellationToken);

            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT sqlite_version();";
            var versao = (string?)await command.ExecuteScalarAsync(cancellationToken) ?? string.Empty;

            return new DatabaseStatusDto(provedor, versao, caminho, File.Exists(caminho), true, null);
        }
        catch (Exception ex)
        {
            return new DatabaseStatusDto(provedor, string.Empty, caminho, File.Exists(caminho), false, ex.Message);
        }
    }
}
