using DeliveryMarketplace.Models;
using DeliveryMarketplace.Repositories;

namespace DeliveryMarketplace.Controllers;

/// <summary>Fase 5: painel do restaurante, cardápio e preparo dos pedidos.</summary>
public class RestauranteController
{
    private readonly RestauranteRepository _restaurantes = new();
    private readonly CategoriaRepository _categorias = new();
    private readonly ProdutoRepository _produtos = new();
    private readonly PedidoRepository _pedidos = new();

    /// <summary>Restaurante do gerente logado.</summary>
    public Restaurante MeuRestaurante() =>
        _restaurantes.BuscarPorGerente(Sessao.Usuario.Id) ?? throw new RegraException("Nenhum restaurante vinculado a esta conta.");

    public void AbrirOuFechar()
    {
        var restaurante = MeuRestaurante();
        if (!restaurante.Ativo)
        {
            throw new RegraException("Restaurante desativado pelo administrador.");
        }

        restaurante.Aberto = !restaurante.Aberto;
        _restaurantes.Atualizar(restaurante);
    }

    public Dictionary<string, string> Indicadores()
    {
        var pedidos = _pedidos.ListarPorRestaurante(MeuRestaurante().Id);
        var entreguesHoje = pedidos.Where(p => p.Status == StatusPedido.Entregue && p.CriadoEm.Date == DateTime.Today).ToList();

        return new Dictionary<string, string>
        {
            ["Pedidos hoje"] = pedidos.Count(p => p.CriadoEm.Date == DateTime.Today).ToString(),
            ["Aguardando ação"] = pedidos.Count(p => p.Status is StatusPedido.AguardandoConfirmacao or StatusPedido.Confirmado or StatusPedido.EmPreparo).ToString(),
            ["Receita líquida hoje"] = entreguesHoje.Sum(p => p.ValorRestaurante).ToString("C"),
            ["Total entregue"] = pedidos.Where(p => p.Status == StatusPedido.Entregue).Sum(p => p.ValorRestaurante).ToString("C")
        };
    }

    // ===== Cardápio =====

    public List<Categoria> ListarCategorias() => _categorias.ListarPorRestaurante(MeuRestaurante().Id);

    public List<Produto> ListarProdutos() => _produtos.ListarPorRestaurante(MeuRestaurante().Id);

    public void CriarCategoria(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new RegraException("Informe o nome da categoria.");
        }

        var restauranteId = MeuRestaurante().Id;
        if (_categorias.ListarPorRestaurante(restauranteId).Any(c => c.Nome.Equals(nome.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new RegraException("Essa categoria já existe.");
        }

        _categorias.Adicionar(new Categoria { Nome = nome.Trim(), RestauranteId = restauranteId });
    }

    /// <summary>Cria (produtoId = 0) ou altera um produto.</summary>
    public void SalvarProduto(int produtoId, string nome, string descricao, decimal preco, Categoria? categoria)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new RegraException("Informe o nome do produto.");
        }

        if (preco <= 0)
        {
            throw new RegraException("O preço deve ser maior que zero.");
        }

        if (categoria is null)
        {
            throw new RegraException("Escolha uma categoria.");
        }

        var restauranteId = MeuRestaurante().Id;
        var produto = produtoId == 0
            ? new Produto { RestauranteId = restauranteId }
            : _produtos.BuscarPorId(produtoId) ?? throw new RegraException("Produto não encontrado.");

        if (produto.RestauranteId != restauranteId)
        {
            throw new RegraException("Este produto não é do seu restaurante.");
        }

        produto.Nome = nome.Trim();
        produto.Descricao = descricao?.Trim() ?? "";
        produto.Preco = preco;
        produto.CategoriaId = categoria.Id;

        if (produtoId == 0)
        {
            _produtos.Adicionar(produto);
        }
        else
        {
            _produtos.Atualizar(produto);
        }
    }

    public void AlternarDisponibilidade(int produtoId)
    {
        var produto = _produtos.BuscarPorId(produtoId) ?? throw new RegraException("Produto não encontrado.");
        produto.Disponivel = !produto.Disponivel;
        _produtos.Atualizar(produto);
    }

    // ===== Pedidos =====

    public List<Pedido> ListarPedidos(bool incluirFinalizados)
    {
        var pedidos = _pedidos.ListarPorRestaurante(MeuRestaurante().Id);
        return incluirFinalizados
            ? pedidos
            : pedidos.Where(p => p.Status != StatusPedido.Entregue && p.Status != StatusPedido.Cancelado).ToList();
    }

    /// <summary>Avança o pedido para a próxima etapa: aceitar → preparar → pronto.</summary>
    public void AvancarPedido(int pedidoId)
    {
        var pedido = PedidoDoRestaurante(pedidoId);
        pedido.Status = pedido.Status switch
        {
            StatusPedido.AguardandoConfirmacao => StatusPedido.Confirmado,
            StatusPedido.Confirmado => StatusPedido.EmPreparo,
            StatusPedido.EmPreparo => StatusPedido.ProntoParaRetirada,
            _ => throw new RegraException($"Nada a fazer: o pedido está \"{pedido.StatusTexto}\".")
        };
        _pedidos.AtualizarStatus(pedido);
    }

    public void RecusarPedido(int pedidoId)
    {
        var pedido = PedidoDoRestaurante(pedidoId);
        new PagamentoController().CancelarPedido(pedido);
    }

    public static string ProximaAcao(StatusPedido status) => status switch
    {
        StatusPedido.AguardandoConfirmacao => "Aceitar",
        StatusPedido.Confirmado => "Iniciar preparo",
        StatusPedido.EmPreparo => "Marcar pronto",
        _ => ""
    };

    private Pedido PedidoDoRestaurante(int pedidoId)
    {
        var pedido = _pedidos.BuscarCompleto(pedidoId) ?? throw new RegraException("Pedido não encontrado.");
        if (pedido.RestauranteId != MeuRestaurante().Id)
        {
            throw new RegraException("Este pedido não é do seu restaurante.");
        }

        return pedido;
    }
}
