using DeliverySystem.Domain.Enums;

namespace DeliverySystem.Application.Interfaces;

public interface INotificationService
{
    Task NotifyPedidoStatusChangedAsync(int pedidoId, PedidoStatus status, CancellationToken ct = default);
}
