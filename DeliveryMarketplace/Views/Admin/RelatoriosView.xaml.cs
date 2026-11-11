using System.Windows;
using System.Windows.Controls;
using DeliveryMarketplace.Controllers;

namespace DeliveryMarketplace.Views.Admin;

public partial class RelatoriosView : UserControl
{
    private readonly RelatorioController _controller = new();

    public RelatoriosView()
    {
        InitializeComponent();
        DtDe.SelectedDate = DateTime.Today.AddDays(-30);
        DtAte.SelectedDate = DateTime.Today;
        Gerar_Click(this, new RoutedEventArgs());
    }

    private DateTime De => DtDe.SelectedDate ?? DateTime.Today;
    private DateTime Ate => DtAte.SelectedDate ?? DateTime.Today;

    private void Gerar_Click(object sender, RoutedEventArgs e)
    {
        Componentes.Executar(() =>
        {
            Componentes.MostrarIndicadores(Indicadores, _controller.Resumo(De, Ate));
            TabRestaurantes.Content = _controller.PorRestaurante(De, Ate);
            TabPagamentos.Content = _controller.PorFormaPagamento(De, Ate);
            TabProdutos.Content = _controller.ProdutosMaisVendidos(De, Ate);
        });
    }

    private void Exportar_Click(object sender, RoutedEventArgs e)
    {
        string caminho = "";
        if (Componentes.Executar(() => caminho = _controller.ExportarCsv(De, Ate)))
        {
            MessageBox.Show($"Relatório salvo em:\n{caminho}", "Exportado", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
