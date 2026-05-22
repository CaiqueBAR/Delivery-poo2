using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Enums;

namespace DeliverySystem.Application.Interfaces;

public interface IPedidoRepository : IRepository<Pedido>
{
    Task<IReadOnlyList<Pedido>> ListByStatusAsync(PedidoStatus status, CancellationToken ct = default);
    Task<IReadOnlyList<Pedido>> ListByDayAsync(DateOnly day, CancellationToken ct = default);
}
