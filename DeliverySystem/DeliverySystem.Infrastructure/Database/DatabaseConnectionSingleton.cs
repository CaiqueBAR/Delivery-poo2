namespace DeliverySystem.Infrastructure.Database;

public sealed class DatabaseConnectionSingleton
{
    private static readonly Lazy<DatabaseConnectionSingleton> Lazy = new(() => new DatabaseConnectionSingleton());

    public static DatabaseConnectionSingleton Instance => Lazy.Value;

    private string _connectionString = string.Empty;

    public string ConnectionString
    {
        get => _connectionString;
        private set => _connectionString = value;
    }

    private DatabaseConnectionSingleton()
    {
    }

    public void Configure(string connectionString)
    {
        ConnectionString = connectionString;
    }
}
