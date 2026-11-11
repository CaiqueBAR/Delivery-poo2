using DeliveryMarketplace.Models;
using DeliveryMarketplace.Repositories;

namespace DeliveryMarketplace.Controllers;

/// <summary>Fase 6: entregas disponíveis, aceite, retirada, entrega e ganhos.</summary>
public class EntregadorController
{
    private readonly UsuarioRepository _usuarios = new();
    private readonly PedidoRepository _pedidos = new();

    public Usuario EuMesmo() => _usuarios.BuscarPorId(Sessao.Usuario.Id) ?? throw new RegraException("Usuário não encontrado.");

    public void FicarDisponivelOuNao()
    {
        var eu = EuMesmo();
        eu.Disponivel = !eu.Disponivel;
        _usuarios.Atualizar(eu);
    }

    /// <summary>Pedidos sem entregador (só aparecem se o entregador estiver disponível).</summary>
    public List<Pedido> EntregasDisponiveis() => EuMesmo().Disponivel ? _pedidos.ListarSemEntregador() : [];

    public List<Pedido> MinhasEntregas() => _pedidos.ListarPorEntregador(Sessao.Usuario.Id);

    public Dictionary<string, string> Ganhos()
    {
        var entregues = MinhasEntregas().Where(p => p.Status == StatusPedido.Entregue).ToList();
        var hoje = entregues.Where(p => (p.EntregueEm ?? p.CriadoEm).Date == DateTime.Today).ToList();

        // O entregador recebe a taxa de entrega de cada pedido.
        return new Dictionary<string, string>
        {
            ["Entregas hoje"] = hoje.Count.ToString(),
            ["Ganhos hoje"] = hoje.Sum(p => p.TaxaEntrega).ToString("C"),
            ["Entregas no total"] = entregues.Count.ToString(),
            ["Ganhos no total"] = entregues.Sum(p => p.TaxaEntrega).ToString("C")
        };
    }

    public void AceitarEntrega(int pedidoId)
    {
        if (!EuMesmo().Disponivel)
        {
            throw new RegraException("Fique disponível para aceitar entregas.");
        }

        if (MinhasEntregas().Any(p => p.Status != StatusPedido.Entregue && p.Status != StatusPedido.Cancelado))
        {
            throw new RegraException("Conclua a entrega atual antes de aceitar outra.");
        }

        var pedido = _pedidos.BuscarCompleto(pedidoId) ?? throw new RegraException("Pedido não encontrado.");
        if (pedido.EntregadorId is not null)
        {
            throw new RegraException("Outro entregador já aceitou este pedido.");
        }

        pedido.EntregadorId = Sessao.Usuario.Id;
        _pedidos.AtualizarStatus(pedido);
    }

    public void ConfirmarRetirada(int pedidoId)
    {
        var pedido = MinhaEntrega(pedidoId);
        if (pedido.Status != StatusPedido.ProntoParaRetirada)
        {
            throw new RegraException("O restaurante ainda não marcou o pedido como pronto.");
        }

        pedido.Status = StatusPedido.EmRota;
        _pedidos.AtualizarStatus(pedido);
    }

    public void ConfirmarEntrega(int pedidoId)
    {
        var pedido = MinhaEntrega(pedidoId);
        if (pedido.Status != StatusPedido.EmRota)
        {
            throw new RegraException("Confirme a retirada antes da entrega.");
        }

        pedido.Status = StatusPedido.Entregue;
        pedido.EntregueEm = DateTime.Now;

        // Pagamento em dinheiro é recebido na entrega.
        new PagamentoController().ConfirmarPagamentoNaEntrega(pedido);
        _pedidos.AtualizarStatus(pedido);
    }

    private Pedido MinhaEntrega(int pedidoId)
    {
        var pedido = _pedidos.BuscarCompleto(pedidoId) ?? throw new RegraException("Pedido não encontrado.");
        if (pedido.EntregadorId != Sessao.Usuario.Id)
        {
            throw new RegraException("Esta entrega não é sua.");
        }

        return pedido;
    }
}
