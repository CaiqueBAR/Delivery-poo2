using DeliverySystem.Application.Interfaces;
using DeliverySystem.Domain.Enums;
using DeliverySystem.Domain.Interfaces;

namespace DeliverySystem.Application.Services.Observers;

public sealed class ClientePedidoObserver : IPedidoObserver
{
    private readonly INotificationService _notifications;

    public ClientePedidoObserver(INotificationService notifications)
    {
        _notifications = notifications;
    }

    public void OnStatusChanged(int pedidoId, PedidoStatus novoStatus)
    {
        _ = _notifications.NotifyPedidoStatusChangedAsync(pedidoId, novoStatus, CancellationToken.None);
    }
}
