using System.Windows.Controls;
using DeliveryMarketplace.Controllers;

namespace DeliveryMarketplace.Views.Admin;

public partial class AdminPainelView : UserControl
{
    private readonly AdminController _controller = new();

    public AdminPainelView()
    {
        InitializeComponent();
        Componentes.MostrarIndicadores(Indicadores, _controller.Indicadores());
        Grade.ItemsSource = _controller.ListarPedidos().Take(10).ToList();
    }
}
