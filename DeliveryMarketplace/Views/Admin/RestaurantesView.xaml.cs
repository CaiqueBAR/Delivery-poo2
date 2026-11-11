using System.Windows;
using System.Windows.Controls;
using DeliveryMarketplace.Controllers;
using DeliveryMarketplace.Models;

namespace DeliveryMarketplace.Views.Admin;

public partial class RestaurantesView : UserControl
{
    private readonly AdminController _controller = new();

    public RestaurantesView()
    {
        InitializeComponent();
        Carregar();
    }

    private void Carregar() => Grade.ItemsSource = _controller.ListarRestaurantes();

    private void Alternar_Click(object sender, RoutedEventArgs e)
    {
        var restaurante = Componentes.Item<Restaurante>(sender);
        if (Componentes.Executar(() => _controller.AtivarOuDesativarRestaurante(restaurante.Id)))
        {
            Carregar();
        }
    }

    private void Cadastrar_Click(object sender, RoutedEventArgs e)
    {
        var ok = Componentes.Executar(() => _controller.CadastrarRestaurante(
                TxtNome.Text, TxtDescricao.Text, TxtEndereco.Text, Componentes.LerValor(TxtTaxa.Text, "Taxa"),
                TxtGerente.Text, TxtEmail.Text),
            "Restaurante cadastrado. O gerente já pode entrar com a senha 123456.");

        if (ok)
        {
            TxtNome.Text = TxtDescricao.Text = TxtEndereco.Text = TxtGerente.Text = TxtEmail.Text = "";
            Carregar();
        }
    }
}
