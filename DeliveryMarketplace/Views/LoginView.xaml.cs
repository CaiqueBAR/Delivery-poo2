using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DeliveryMarketplace.Controllers;

namespace DeliveryMarketplace.Views;

public partial class LoginView : UserControl
{
    private readonly MainWindow _janela;
    private readonly AuthController _controller = new();

    public LoginView(MainWindow janela)
    {
        InitializeComponent();
        _janela = janela;

        var demos = new[]
        {
            ("Administrador", "admin@delivery.local"),
            ("Restaurante", "pizzaria@delivery.local"),
            ("Entregador", "joao@delivery.local"),
            ("Cliente", "fernanda@delivery.local")
        };

        foreach (var (perfil, email) in demos)
        {
            var botao = new Button { Content = perfil, Style = (Style)FindResource("SecondaryButton"), Margin = new Thickness(0, 0, 8, 8) };
            botao.Click += (_, _) =>
            {
                TxtEmail.Text = email;
                TxtSenha.Password = "123456";
            };
            Demos.Children.Add(botao);
        }
    }

    private void Entrar_Click(object sender, RoutedEventArgs e)
    {
        Componentes.Executar(() =>
        {
            var usuario = _controller.Entrar(TxtEmail.Text, TxtSenha.Password);
            _janela.AposLogin(usuario);
        });
    }

    private void Senha_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            Entrar_Click(sender, e);
        }
    }

    private void Cadastrar_Click(object sender, RoutedEventArgs e)
    {
        var ok = Componentes.Executar(
            () => _controller.CadastrarCliente(CadNome.Text, CadEmail.Text, CadSenha.Password, CadTelefone.Text, CadCpf.Text, CadEndereco.Text),
            "Conta criada! Agora é só entrar.");

        if (ok)
        {
            TxtEmail.Text = CadEmail.Text.Trim().ToLower();
            TxtSenha.Password = "";
        }
    }
}
