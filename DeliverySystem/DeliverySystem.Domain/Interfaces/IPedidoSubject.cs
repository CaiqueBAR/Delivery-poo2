namespace DeliverySystem.Domain.Interfaces;

public interface IPedidoSubject
{
    void Subscribe(IPedidoObserver observer);
    void Unsubscribe(IPedidoObserver observer);
}
