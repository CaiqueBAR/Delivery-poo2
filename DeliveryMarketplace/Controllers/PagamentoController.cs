using DeliveryMarketplace.Models;
using DeliveryMarketplace.Repositories;

namespace DeliveryMarketplace.Controllers;

/// <summary>
/// Fase 8: pagamento SIMULADO (nenhum dado sai do computador).
/// - Pix: aprovado na hora.
/// - Cartão: precisa ter 16 dígitos; cartões terminados em 0000 são recusados (para testar a recusa).
/// - Dinheiro: fica pendente e é aprovado quando o entregador confirma a entrega.
/// </summary>
public class PagamentoController
{
    private readonly PedidoRepository _pedidos = new();

    public Pagamento Processar(MetodoPagamento metodo, decimal valor, string? numeroCartao)
    {
        var pagamento = new Pagamento { Metodo = metodo, Valor = valor };

        switch (metodo)
        {
            case MetodoPagamento.Pix:
                Aprovar(pagamento, "PIX");
                break;

            case MetodoPagamento.CartaoCredito:
            case MetodoPagamento.CartaoDebito:
                var digitos = new string((numeroCartao ?? "").Where(char.IsDigit).ToArray());
                if (digitos.Length != 16)
                {
                    throw new RegraException("Pagamento recusado: o cartão deve ter 16 dígitos.");
                }

                if (digitos.EndsWith("0000"))
                {
                    throw new RegraException("Pagamento recusado pela operadora (simulação).");
                }

                Aprovar(pagamento, "CARD");
                break;

            case MetodoPagamento.Dinheiro:
                pagamento.Status = StatusPagamento.Pendente;
                break;
        }

        return pagamento;
    }

    public void ConfirmarPagamentoNaEntrega(Pedido pedido)
    {
        if (pedido.Pagamento is { Status: StatusPagamento.Pendente })
        {
            Aprovar(pedido.Pagamento, "DIN");
        }
    }

    /// <summary>Cancela o pedido e estorna o pagamento, se já tiver sido aprovado.</summary>
    public void CancelarPedido(Pedido pedido)
    {
        if (pedido.Status is StatusPedido.EmRota or StatusPedido.Entregue or StatusPedido.Cancelado)
        {
            throw new RegraException($"Não é possível cancelar: o pedido está \"{pedido.StatusTexto}\".");
        }

        pedido.Status = StatusPedido.Cancelado;
        if (pedido.Pagamento is not null)
        {
            pedido.Pagamento.Status = pedido.Pagamento.Status == StatusPagamento.Aprovado
                ? StatusPagamento.Estornado
                : StatusPagamento.Recusado;
        }

        _pedidos.AtualizarStatus(pedido);
    }

    private static void Aprovar(Pagamento pagamento, string prefixo)
    {
        pagamento.Status = StatusPagamento.Aprovado;
        pagamento.CodigoTransacao = $"{prefixo}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
    }
}
