using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Interfaces;

namespace DeliverySystem.Domain.Patterns;

public sealed class PromocaoHorarioStrategy : IPedidoPricingStrategy
{
    private readonly TimeSpan _inicio;
    private readonly TimeSpan _fim;
    private readonly decimal _percentualDesconto;

    public PromocaoHorarioStrategy(TimeSpan inicio, TimeSpan fim, decimal percentualDesconto)
    {
        _inicio = inicio;
        _fim = fim;
        _percentualDesconto = percentualDesconto;
    }

    public decimal Apply(Pedido pedido, decimal valorAtual)
    {
        var agora = DateTime.UtcNow.TimeOfDay;
        var dentroDaJanela = _inicio <= _fim
            ? agora >= _inicio && agora <= _fim
            : agora >= _inicio || agora <= _fim;

        if (!dentroDaJanela) return valorAtual;

        return Math.Max(0m, valorAtual - (valorAtual * (_percentualDesconto / 100m)));
    }
}
