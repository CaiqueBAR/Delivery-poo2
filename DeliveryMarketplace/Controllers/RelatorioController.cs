using System.Text;
using DeliveryMarketplace.Models;
using DeliveryMarketplace.Repositories;

namespace DeliveryMarketplace.Controllers;

/// <summary>Linha de uma tabela de relatório.</summary>
public record LinhaRelatorio(string Descricao, int Quantidade, decimal Valor);

/// <summary>Fase 9: relatórios por período e exportação para CSV.</summary>
public class RelatorioController
{
    private readonly PedidoRepository _pedidos = new();

    public List<Pedido> PedidosEntregues(DateTime de, DateTime ate)
    {
        if (ate.Date < de.Date)
        {
            throw new RegraException("A data final deve ser depois da inicial.");
        }

        return _pedidos.ListarPorPeriodo(de, ate).Where(p => p.Status == StatusPedido.Entregue).ToList();
    }

    public Dictionary<string, string> Resumo(DateTime de, DateTime ate)
    {
        var todos = _pedidos.ListarPorPeriodo(de, ate);
        var entregues = todos.Where(p => p.Status == StatusPedido.Entregue).ToList();
        var bruto = entregues.Sum(p => p.Total);

        return new Dictionary<string, string>
        {
            ["Pedidos no período"] = todos.Count.ToString(),
            ["Entregues"] = entregues.Count.ToString(),
            ["Cancelados"] = todos.Count(p => p.Status == StatusPedido.Cancelado).ToString(),
            ["Faturamento bruto"] = bruto.ToString("C"),
            ["Comissão da plataforma"] = entregues.Sum(p => p.Comissao).ToString("C"),
            ["Repasse restaurantes"] = entregues.Sum(p => p.ValorRestaurante).ToString("C"),
            ["Repasse entregadores"] = entregues.Sum(p => p.TaxaEntrega).ToString("C"),
            ["Ticket médio"] = (entregues.Count == 0 ? 0 : bruto / entregues.Count).ToString("C")
        };
    }

    public List<LinhaRelatorio> PorRestaurante(DateTime de, DateTime ate) =>
        PedidosEntregues(de, ate).GroupBy(p => p.RestauranteNome)
            .Select(g => new LinhaRelatorio(g.Key, g.Count(), g.Sum(p => p.Total)))
            .OrderByDescending(l => l.Valor).ToList();

    public List<LinhaRelatorio> PorFormaPagamento(DateTime de, DateTime ate) =>
        PedidosEntregues(de, ate).Where(p => p.Pagamento is not null).GroupBy(p => p.Pagamento!.Metodo.ToString())
            .Select(g => new LinhaRelatorio(g.Key, g.Count(), g.Sum(p => p.Total)))
            .OrderByDescending(l => l.Valor).ToList();

    public List<LinhaRelatorio> ProdutosMaisVendidos(DateTime de, DateTime ate) =>
        PedidosEntregues(de, ate).SelectMany(p => p.Itens).GroupBy(i => i.NomeProduto)
            .Select(g => new LinhaRelatorio(g.Key, g.Sum(i => i.Quantidade), g.Sum(i => i.Subtotal)))
            .OrderByDescending(l => l.Quantidade).Take(10).ToList();

    /// <summary>Salva o relatório em CSV (abre no Excel) na pasta Documentos e devolve o caminho.</summary>
    public string ExportarCsv(DateTime de, DateTime ate)
    {
        var csv = new StringBuilder();
        csv.AppendLine($"Relatório DeliveryMarketplace;{de:dd/MM/yyyy} a {ate:dd/MM/yyyy}");
        csv.AppendLine();
        foreach (var (nome, valor) in Resumo(de, ate))
        {
            csv.AppendLine($"{nome};{valor}");
        }

        Secao("Vendas por restaurante", PorRestaurante(de, ate));
        Secao("Vendas por forma de pagamento", PorFormaPagamento(de, ate));
        Secao("Produtos mais vendidos", ProdutosMaisVendidos(de, ate));

        var pasta = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var caminho = Path.Combine(pasta, $"relatorio-delivery-{DateTime.Now:yyyyMMdd-HHmmss}.csv");
        File.WriteAllText(caminho, csv.ToString(), new UTF8Encoding(true));
        return caminho;

        void Secao(string titulo, List<LinhaRelatorio> linhas)
        {
            csv.AppendLine();
            csv.AppendLine(titulo);
            csv.AppendLine("Descrição;Quantidade;Valor");
            foreach (var l in linhas)
            {
                csv.AppendLine($"{l.Descricao};{l.Quantidade};{l.Valor:N2}");
            }
        }
    }
}
