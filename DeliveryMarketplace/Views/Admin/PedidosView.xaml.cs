using System.Windows.Controls;
using DeliveryMarketplace.Controllers;
using DeliveryMarketplace.Models;

namespace DeliveryMarketplace.Views.Admin;

public partial class PedidosView : UserControl
{
    private readonly AdminController _controller = new();

    public PedidosView()
    {
        InitializeComponent();
        CmbStatus.Items.Add("Todos");
        foreach (var status in Enum.GetValues<StatusPedido>())
        {
            CmbStatus.Items.Add(status);
        }

        CmbStatus.SelectedIndex = 0;
    }

    private void Filtro_Changed(object sender, SelectionChangedEventArgs e)
    {
        var filtro = CmbStatus.SelectedItem as StatusPedido?;
        Grade.ItemsSource = _controller.ListarPedidos(filtro);
    }
}
