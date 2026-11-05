using Microsoft.Data.Sqlite;

namespace DeliveryMarketplace.Infrastructure.Data;

/// <summary>
/// Converte o caminho relativo do arquivo SQLite (ex.: "Data Source=delivery.db")
/// em caminho absoluto. Assim o banco fica sempre ao lado do executável,
/// independentemente da pasta de onde a aplicação foi iniciada.
/// </summary>
public static class SqliteConnectionStringResolver
{
    public const string DefaultConnectionString = "Data Source=delivery.db";

    public static string Resolve(string? connectionString, string baseDirectory)
    {
        var builder = new SqliteConnectionStringBuilder(
            string.IsNullOrWhiteSpace(connectionString) ? DefaultConnectionString : connectionString);

        var dataSource = builder.DataSource;
        var isMemory = dataSource.Equals(":memory:", StringComparison.OrdinalIgnoreCase)
                       || builder.Mode == SqliteOpenMode.Memory;

        if (!isMemory && !Path.IsPathRooted(dataSource))
        {
            builder.DataSource = Path.GetFullPath(Path.Combine(baseDirectory, dataSource));
        }

        return builder.ToString();
    }
}
