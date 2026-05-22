using DeliverySystem.Application.Interfaces;
using DeliverySystem.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace DeliverySystem.Infrastructure.Repositories;

public class EfRepository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly DeliveryDbContext Db;

    public EfRepository(DeliveryDbContext db)
    {
        Db = db;
    }

    public virtual Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return Db.Set<TEntity>().FindAsync([id], ct).AsTask();
    }

    public virtual async Task<IReadOnlyList<TEntity>> ListAsync(CancellationToken ct = default)
    {
        return await Db.Set<TEntity>().AsNoTracking().ToListAsync(ct);
    }

    public virtual async Task AddAsync(TEntity entity, CancellationToken ct = default)
    {
        await Db.Set<TEntity>().AddAsync(entity, ct);
    }

    public virtual void Update(TEntity entity)
    {
        Db.Set<TEntity>().Update(entity);
    }

    public virtual void Remove(TEntity entity)
    {
        Db.Set<TEntity>().Remove(entity);
    }
}
