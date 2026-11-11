namespace DeliveryMarketplace.Models;

public class Pedido
{
    /// <summary>Comissão da plataforma sobre o valor dos itens.</summary>
    public const decimal PercentualComissao = 0.12m;

    public int Id { get; set; }
    public StatusPedido Status { get; set; } = StatusPedido.AguardandoConfirmacao;
    public string EnderecoEntrega { get; set; } = "";
    public string? Observacao { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxaEntrega { get; set; }
    public decimal Total { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.Now;
    public DateTime? EntregueEm { get; set; }

    public int ClienteId { get; set; }
    public Usuario? Cliente { get; set; }

    public int RestauranteId { get; set; }
    public Restaurante? Restaurante { get; set; }

    public int? EntregadorId { get; set; }
    public Usuario? Entregador { get; set; }

    public List<ItemPedido> Itens { get; set; } = [];
    public Pagamento? Pagamento { get; set; }

    // Propriedades calculadas (não vão para o banco)
    public decimal Comissao => Math.Round(Subtotal * PercentualComissao, 2);
    public decimal ValorRestaurante => Subtotal - Comissao;
    public string Codigo => $"#{Id:D5}";
    public string ClienteNome => Cliente?.Nome ?? "";
    public string RestauranteNome => Restaurante?.Nome ?? "";
    public string EntregadorNome => Entregador?.Nome ?? "—";
    public string StatusTexto => Status switch
    {
        StatusPedido.AguardandoConfirmacao => "Aguardando confirmação",
        StatusPedido.Confirmado => "Confirmado",
        StatusPedido.EmPreparo => "Em preparo",
        StatusPedido.ProntoParaRetirada => "Pronto para retirada",
        StatusPedido.EmRota => "Saiu para entrega",
        StatusPedido.Entregue => "Entregue",
        _ => "Cancelado"
    };
    public string ItensTexto => string.Join(", ", Itens.Select(i => $"{i.Quantidade}x {i.NomeProduto}"));
    public string PagamentoTexto => Pagamento is null ? "—" : $"{Pagamento.Metodo} · {Pagamento.Status}";
}

public class ItemPedido
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public int ProdutoId { get; set; }

    // Nome e preço copiados no momento da compra: o histórico não muda se o produto mudar.
    public string NomeProduto { get; set; } = "";
    public decimal PrecoUnitario { get; set; }
    public int Quantidade { get; set; }

    public decimal Subtotal => PrecoUnitario * Quantidade;
}

public class Pagamento
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public MetodoPagamento Metodo { get; set; }
    public StatusPagamento Status { get; set; } = StatusPagamento.Pendente;
    public decimal Valor { get; set; }
    public string? CodigoTransacao { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.Now;
}
