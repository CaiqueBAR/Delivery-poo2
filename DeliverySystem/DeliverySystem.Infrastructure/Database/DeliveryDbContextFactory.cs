using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DeliverySystem.Infrastructure.Database;

public sealed class DeliveryDbContextFactory : IDesignTimeDbContextFactory<DeliveryDbContext>
{
    public DeliveryDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=DeliverySystemDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

        var optionsBuilder = new DbContextOptionsBuilder<DeliveryDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new DeliveryDbContext(optionsBuilder.Options);
    }
}
