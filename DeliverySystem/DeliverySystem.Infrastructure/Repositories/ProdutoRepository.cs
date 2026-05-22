using DeliverySystem.Application.Interfaces;
using DeliverySystem.Domain.Entities;
using DeliverySystem.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace DeliverySystem.Infrastructure.Repositories;

public sealed class ProdutoRepository : EfRepository<Produto>, IProdutoRepository
{
    public ProdutoRepository(DeliveryDbContext db) : base(db)
    {
    }

    public async Task<IReadOnlyList<Produto>> ListByRestauranteAsync(int restauranteId, CancellationToken ct = default)
    {
        return await Db.Produtos
            .AsNoTracking()
            .Where(p => p.RestauranteId == restauranteId)
            .OrderBy(p => p.Nome)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Produto>> SearchByNomeAsync(string termo, CancellationToken ct = default)
    {
        termo = termo.Trim();
        return await Db.Produtos
            .AsNoTracking()
            .Where(p => p.Nome.Contains(termo))
            .OrderBy(p => p.Nome)
            .ToListAsync(ct);
    }

    public override Task<Produto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return Db.Produtos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }
}
