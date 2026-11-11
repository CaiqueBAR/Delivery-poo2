using DeliveryMarketplace.Models;
using DeliveryMarketplace.Repositories;

namespace DeliveryMarketplace.Controllers;

/// <summary>Item do carrinho (fica só na memória até o pedido ser finalizado).</summary>
public class ItemCarrinho
{
    public Produto Produto { get; set; } = null!;
    public int Quantidade { get; set; }
    public string Nome => Produto.Nome;
    public decimal Subtotal => Produto.Preco * Quantidade;
}

/// <summary>Fase 7: vitrine de restaurantes, carrinho, pedido e acompanhamento.</summary>
public class ClienteController
{
    private readonly RestauranteRepository _restaurantes = new();
    private readonly ProdutoRepository _produtos = new();
    private readonly PedidoRepository _pedidos = new();
    private readonly UsuarioRepository _usuarios = new();

    public List<Restaurante> ListarRestaurantes() => _restaurantes.ListarAtivos();

    public List<Produto> Cardapio(int restauranteId) => _produtos.ListarPorRestaurante(restauranteId, somenteDisponiveis: true);

    public void AdicionarAoCarrinho(List<ItemCarrinho> carrinho, Produto produto)
    {
        if (carrinho.Count > 0 && carrinho[0].Produto.RestauranteId != produto.RestauranteId)
        {
            throw new RegraException("O carrinho só pode ter itens de um restaurante. Esvazie-o para trocar.");
        }

        var item = carrinho.FirstOrDefault(i => i.Produto.Id == produto.Id);
        if (item is null)
        {
            carrinho.Add(new ItemCarrinho { Produto = produto, Quantidade = 1 });
        }
        else
        {
            item.Quantidade++;
        }
    }

    /// <summary>Finaliza o pedido. O pagamento é processado antes de gravar: se for recusado, nada é salvo.</summary>
    public Pedido FazerPedido(Restaurante restaurante, List<ItemCarrinho> carrinho, MetodoPagamento metodo,
        string? numeroCartao, string? observacao)
    {
        if (carrinho.Count == 0)
        {
            throw new RegraException("Seu carrinho está vazio.");
        }

        var atual = _restaurantes.BuscarPorId(restaurante.Id);
        if (atual is null || !atual.Ativo || !atual.Aberto)
        {
            throw new RegraException($"{restaurante.Nome} está fechado no momento.");
        }

        var cliente = _usuarios.BuscarPorId(Sessao.Usuario.Id)!;
        var pedido = new Pedido
        {
            ClienteId = cliente.Id,
            RestauranteId = atual.Id,
            EnderecoEntrega = cliente.Endereco ?? "",
            Observacao = string.IsNullOrWhiteSpace(observacao) ? null : observacao.Trim(),
            TaxaEntrega = atual.TaxaEntrega,
            Itens = carrinho.Select(i => new ItemPedido
            {
                ProdutoId = i.Produto.Id,
                NomeProduto = i.Produto.Nome,
                PrecoUnitario = i.Produto.Preco,
                Quantidade = i.Quantidade
            }).ToList()
        };
        pedido.Subtotal = pedido.Itens.Sum(i => i.Subtotal);
        pedido.Total = pedido.Subtotal + pedido.TaxaEntrega;

        // Fase 8: pagamento simulado
        pedido.Pagamento = new PagamentoController().Processar(metodo, pedido.Total, numeroCartao);

        _pedidos.Criar(pedido);
        return pedido;
    }

    public List<Pedido> MeusPedidos() => _pedidos.ListarPorCliente(Sessao.Usuario.Id);

    public void CancelarPedido(int pedidoId)
    {
        var pedido = _pedidos.BuscarCompleto(pedidoId) ?? throw new RegraException("Pedido não encontrado.");
        if (pedido.ClienteId != Sessao.Usuario.Id)
        {
            throw new RegraException("Este pedido não é seu.");
        }

        if (pedido.Status != StatusPedido.AguardandoConfirmacao)
        {
            throw new RegraException("O restaurante já aceitou o pedido; não é mais possível cancelar.");
        }

        new PagamentoController().CancelarPedido(pedido);
    }
}
