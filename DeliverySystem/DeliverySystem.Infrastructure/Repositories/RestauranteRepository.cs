using DeliverySystem.Application.Interfaces;
using DeliverySystem.Domain.Entities;
using DeliverySystem.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace DeliverySystem.Infrastructure.Repositories;

public sealed class RestauranteRepository : EfRepository<Restaurante>, IRestauranteRepository
{
    public RestauranteRepository(DeliveryDbContext db) : base(db)
    {
    }

    public async Task<IReadOnlyList<Restaurante>> SearchByNomeAsync(string termo, CancellationToken ct = default)
    {
        termo = termo.Trim();
        return await Db.Restaurantes
            .AsNoTracking()
            .Where(r => r.Nome.Contains(termo))
            .OrderBy(r => r.Nome)
            .ToListAsync(ct);
    }
}
