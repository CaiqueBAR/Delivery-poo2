using DeliverySystem.Domain.Entities;

namespace DeliverySystem.Domain.Interfaces;

public interface IPedidoPricingStrategy
{
    decimal Apply(Pedido pedido, decimal valorAtual);
}
