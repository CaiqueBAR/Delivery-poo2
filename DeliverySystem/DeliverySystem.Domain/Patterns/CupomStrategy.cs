using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Enums;
using DeliverySystem.Domain.Interfaces;

namespace DeliverySystem.Domain.Patterns;

public sealed class CupomStrategy : IPedidoPricingStrategy
{
    private readonly Cupom _cupom;

    public CupomStrategy(Cupom cupom)
    {
        _cupom = cupom;
    }

    public decimal Apply(Pedido pedido, decimal valorAtual)
    {
        if (!_cupom.Ativo) return valorAtual;

        return _cupom.Tipo switch
        {
            CupomTipo.FreteGratis => valorAtual,
            CupomTipo.ValorFixo => Math.Max(0m, valorAtual - _cupom.Valor),
            CupomTipo.Percentual => Math.Max(0m, valorAtual - (valorAtual * (_cupom.Valor / 100m))),
            _ => valorAtual
        };
    }
}
