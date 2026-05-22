using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Interfaces;

namespace DeliverySystem.Domain.Patterns;

public sealed class TaxaEntregaStrategy : IPedidoPricingStrategy
{
    private readonly decimal _taxaEntrega;

    public TaxaEntregaStrategy(decimal taxaEntrega)
    {
        _taxaEntrega = taxaEntrega;
    }

    public decimal Apply(Pedido pedido, decimal valorAtual)
    {
        return valorAtual + _taxaEntrega;
    }
}
