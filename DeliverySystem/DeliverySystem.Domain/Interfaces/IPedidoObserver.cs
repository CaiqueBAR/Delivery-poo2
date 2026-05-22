using DeliverySystem.Domain.Enums;

namespace DeliverySystem.Domain.Interfaces;

public interface IPedidoObserver
{
    void OnStatusChanged(int pedidoId, PedidoStatus novoStatus);
}
