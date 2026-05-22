using DeliverySystem.Application.Interfaces;
using DeliverySystem.Domain.Enums;

namespace DeliverySystem.Infrastructure.Services;

public sealed class InMemoryNotificationService : INotificationService
{
    private readonly List<(int PedidoId, PedidoStatus Status, DateTime UtcAt)> _events = new();

    public Task NotifyPedidoStatusChangedAsync(int pedidoId, PedidoStatus status, CancellationToken ct = default)
    {
        _events.Add((pedidoId, status, DateTime.UtcNow));
        return Task.CompletedTask;
    }

    public IReadOnlyList<(int PedidoId, PedidoStatus Status, DateTime UtcAt)> ListEvents()
    {
        return _events;
    }
}
