using System.Windows;
using System.Windows.Controls;
using DeliveryMarketplace.Controllers;
using DeliveryMarketplace.Models;

namespace DeliveryMarketplace.Views.Admin;

public partial class UsuariosView : UserControl
{
    private readonly AdminController _controller = new();

    public UsuariosView()
    {
        InitializeComponent();
        Carregar();
    }

    private void Carregar() => Grade.ItemsSource = _controller.ListarUsuarios();

    private void Alternar_Click(object sender, RoutedEventArgs e)
    {
        var usuario = Componentes.Item<Usuario>(sender);
        if (Componentes.Executar(() => _controller.AtivarOuDesativarUsuario(usuario.Id)))
        {
            Carregar();
        }
    }

    private void Cadastrar_Click(object sender, RoutedEventArgs e)
    {
        if (Componentes.Executar(() => _controller.CadastrarEntregador(TxtNome.Text, TxtEmail.Text, TxtTelefone.Text, TxtVeiculo.Text),
                "Entregador cadastrado."))
        {
            TxtNome.Text = TxtEmail.Text = TxtTelefone.Text = TxtVeiculo.Text = "";
            Carregar();
        }
    }
}
