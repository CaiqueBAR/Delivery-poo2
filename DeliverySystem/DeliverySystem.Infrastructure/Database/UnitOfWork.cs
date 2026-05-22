using DeliverySystem.Application.Interfaces;

namespace DeliverySystem.Infrastructure.Database;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly DeliveryDbContext _db;

    public UnitOfWork(DeliveryDbContext db)
    {
        _db = db;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return _db.SaveChangesAsync(ct);
    }
}
