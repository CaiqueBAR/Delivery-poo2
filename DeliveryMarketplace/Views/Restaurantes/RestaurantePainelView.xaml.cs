using System.Windows;
using System.Windows.Controls;
using DeliveryMarketplace.Controllers;
using DeliveryMarketplace.Models;

namespace DeliveryMarketplace.Views.Restaurantes;

public partial class RestaurantePainelView : UserControl
{
    private readonly RestauranteController _controller = new();

    public RestaurantePainelView()
    {
        InitializeComponent();
        Componentes.Executar(Carregar);
    }

    private void Carregar()
    {
        var restaurante = _controller.MeuRestaurante();
        TxtNome.Text = restaurante.Nome;
        TxtSituacao.Text = $"{restaurante.SituacaoTexto} · taxa de entrega {restaurante.TaxaEntrega:C} · {restaurante.TempoEntregaMinutos} min";
        BtnAbrir.Content = restaurante.Aberto ? "Fechar restaurante" : "Abrir restaurante";

        Componentes.MostrarIndicadores(Indicadores, _controller.Indicadores());
        Grade.ItemsSource = _controller.ListarPedidos(ChkFinalizados.IsChecked == true);
    }

    private void Abrir_Click(object sender, RoutedEventArgs e)
    {
        if (Componentes.Executar(_controller.AbrirOuFechar))
        {
            Carregar();
        }
    }

    private void Atualizar_Click(object sender, RoutedEventArgs e) => Componentes.Executar(Carregar);

    private void Avancar_Click(object sender, RoutedEventArgs e)
    {
        var pedido = Componentes.Item<Pedido>(sender);
        if (Componentes.Executar(() => _controller.AvancarPedido(pedido.Id)))
        {
            Carregar();
        }
    }

    private void Recusar_Click(object sender, RoutedEventArgs e)
    {
        var pedido = Componentes.Item<Pedido>(sender);
        if (MessageBox.Show($"Recusar/cancelar o pedido {pedido.Codigo}? O pagamento será estornado.", "Confirmar",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
        {
            return;
        }

        if (Componentes.Executar(() => _controller.RecusarPedido(pedido.Id)))
        {
            Carregar();
        }
    }
}
