using DeliverySystem.Application.Interfaces;
using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Enums;
using DeliverySystem.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace DeliverySystem.Infrastructure.Repositories;

public sealed class PedidoRepository : EfRepository<Pedido>, IPedidoRepository
{
    public PedidoRepository(DeliveryDbContext db) : base(db)
    {
    }

    public override Task<Pedido?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return Db.Pedidos
            .Include(p => p.Itens)
            .ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public override async Task<IReadOnlyList<Pedido>> ListAsync(CancellationToken ct = default)
    {
        return await Db.Pedidos
            .AsNoTracking()
            .Include(p => p.Itens)
            .ThenInclude(i => i.Produto)
            .OrderByDescending(p => p.DataPedido)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Pedido>> ListByStatusAsync(PedidoStatus status, CancellationToken ct = default)
    {
        return await Db.Pedidos
            .AsNoTracking()
            .Where(p => p.Status == status)
            .Include(p => p.Itens)
            .ThenInclude(i => i.Produto)
            .OrderByDescending(p => p.DataPedido)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Pedido>> ListByDayAsync(DateOnly day, CancellationToken ct = default)
    {
        var start = day.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var end = day.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

        return await Db.Pedidos
            .AsNoTracking()
            .Where(p => p.DataPedido >= start && p.DataPedido <= end)
            .OrderByDescending(p => p.DataPedido)
            .ToListAsync(ct);
    }
}
