using System.Windows;
using System.Windows.Controls;
using DeliveryMarketplace.Controllers;
using DeliveryMarketplace.Models;

namespace DeliveryMarketplace.Views.Clientes;

public partial class MeusPedidosView : UserControl
{
    private readonly ClienteController _controller = new();

    public MeusPedidosView()
    {
        InitializeComponent();
        Carregar();
    }

    private void Carregar() => Grade.ItemsSource = _controller.MeusPedidos();

    private void Atualizar_Click(object sender, RoutedEventArgs e) => Carregar();

    private void Cancelar_Click(object sender, RoutedEventArgs e)
    {
        var pedido = Componentes.Item<Pedido>(sender);
        if (Componentes.Executar(() => _controller.CancelarPedido(pedido.Id), $"Pedido {pedido.Codigo} cancelado."))
        {
            Carregar();
        }
    }
}
