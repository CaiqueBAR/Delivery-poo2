using DeliverySystem.Application.Interfaces;

namespace DeliverySystem.Application.Services;

public sealed class RelatorioAppService
{
    private readonly IPedidoRepository _pedidos;

    public RelatorioAppService(IPedidoRepository pedidos)
    {
        _pedidos = pedidos;
    }

    public async Task<decimal> ValorVendidoNoDiaAsync(DateOnly dia, CancellationToken ct = default)
    {
        var list = await _pedidos.ListByDayAsync(dia, ct);
        return list.Sum(p => p.ValorTotal);
    }

    public async Task<decimal> ValorVendidoNoMesAsync(int ano, int mes, CancellationToken ct = default)
    {
        var all = await _pedidos.ListAsync(ct);
        return all
            .Where(p => p.DataPedido.Year == ano && p.DataPedido.Month == mes)
            .Sum(p => p.ValorTotal);
    }
}
