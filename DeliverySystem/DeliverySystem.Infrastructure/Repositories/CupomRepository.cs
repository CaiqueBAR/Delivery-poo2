using DeliverySystem.Application.Interfaces;
using DeliverySystem.Domain.Entities;
using DeliverySystem.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace DeliverySystem.Infrastructure.Repositories;

public sealed class CupomRepository : EfRepository<Cupom>, ICupomRepository
{
    public CupomRepository(DeliveryDbContext db) : base(db)
    {
    }

    public Task<Cupom?> GetByCodigoAsync(string codigo, CancellationToken ct = default)
    {
        codigo = codigo.Trim().ToUpperInvariant();
        return Db.Cupons.FirstOrDefaultAsync(c => c.Codigo == codigo, ct);
    }
}
