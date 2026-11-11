using System.Windows;
using System.Windows.Controls;
using DeliveryMarketplace.Controllers;
using DeliveryMarketplace.Models;
using DeliveryMarketplace.Views.Admin;
using DeliveryMarketplace.Views.Clientes;
using DeliveryMarketplace.Views.Entregadores;
using DeliveryMarketplace.Views.Restaurantes;

namespace DeliveryMarketplace.Views;

/// <summary>
/// Janela principal: menu lateral (muda conforme o perfil) e área de conteúdo onde as telas são exibidas.
/// </summary>
public partial class MainWindow : Window
{
    private readonly List<(Button botao, Func<UserControl> criarTela, string titulo, string subtitulo)> _menu = [];

    public MainWindow()
    {
        InitializeComponent();
        MostrarLogin();
    }

    /// <summary>Tela de login: esconde o menu.</summary>
    public void MostrarLogin()
    {
        Sessao.Sair();
        ColunaMenu.Width = new GridLength(0);
        Menu.Visibility = Visibility.Collapsed;
        Cabecalho.Visibility = Visibility.Collapsed;
        Conteudo.Content = new LoginView(this);
    }

    /// <summary>Chamado pelo login: monta o menu do perfil e abre a primeira tela.</summary>
    public void AposLogin(Usuario usuario)
    {
        ColunaMenu.Width = new GridLength(248);
        Menu.Visibility = Visibility.Visible;
        Cabecalho.Visibility = Visibility.Visible;
        TxtUsuario.Text = usuario.Nome;
        TxtPerfil.Text = usuario.Perfil.ToString();

        _menu.Clear();
        ItensMenu.Children.Clear();

        switch (usuario.Perfil)
        {
            case Perfil.Administrador:
                AdicionarMenu("", "Painel", "Visão geral da plataforma", () => new AdminPainelView());
                AdicionarMenu("", "Usuários", "Clientes, entregadores e gerentes", () => new UsuariosView());
                AdicionarMenu("", "Restaurantes", "Parceiros do marketplace", () => new RestaurantesView());
                AdicionarMenu("", "Pedidos", "Todos os pedidos e pagamentos", () => new PedidosView());
                AdicionarMenu("", "Relatórios", "Faturamento, comissões e mais vendidos", () => new RelatoriosView());
                AdicionarMenu("", "Banco de dados", "Arquivo delivery.db e tabelas", () => new BancoDadosView());
                break;

            case Perfil.Restaurante:
                AdicionarMenu("", "Painel e pedidos", "Abra o restaurante e acompanhe os pedidos", () => new RestaurantePainelView());
                AdicionarMenu("", "Cardápio", "Categorias e produtos", () => new CardapioView());
                break;

            case Perfil.Entregador:
                AdicionarMenu("", "Entregas", "Aceite, retire e entregue pedidos", () => new EntregadorView());
                break;

            case Perfil.Cliente:
                AdicionarMenu("", "Fazer pedido", "Escolha um restaurante e monte seu carrinho", () => new ComprarView());
                AdicionarMenu("", "Meus pedidos", "Acompanhe o status dos seus pedidos", () => new MeusPedidosView());
                break;
        }

        Navegar(0);
    }

    private void AdicionarMenu(string icone, string titulo, string subtitulo, Func<UserControl> criarTela)
    {
        var texto = new StackPanel { Orientation = Orientation.Horizontal };
        texto.Children.Add(new TextBlock { Text = icone, Style = (Style)FindResource("IconText"), FontSize = 16, Margin = new Thickness(0, 0, 12, 0) });
        texto.Children.Add(new TextBlock { Text = titulo, FontSize = 14, VerticalAlignment = VerticalAlignment.Center });

        var botao = new Button { Content = texto, Style = (Style)FindResource("NavButton") };
        System.Windows.Automation.AutomationProperties.SetName(botao, titulo);
        var indice = _menu.Count;
        botao.Click += (_, _) => Navegar(indice);

        _menu.Add((botao, criarTela, titulo, subtitulo));
        ItensMenu.Children.Add(botao);
    }

    private void Navegar(int indice)
    {
        for (var i = 0; i < _menu.Count; i++)
        {
            var selecionado = i == indice;
            _menu[i].botao.Background = selecionado ? (System.Windows.Media.Brush)FindResource("PrimaryBrush") : System.Windows.Media.Brushes.Transparent;
            _menu[i].botao.Foreground = selecionado ? System.Windows.Media.Brushes.White : (System.Windows.Media.Brush)FindResource("SidebarTextBrush");
        }

        var item = _menu[indice];
        TxtTitulo.Text = item.titulo;
        TxtSubtitulo.Text = item.subtitulo;
        Conteudo.Content = item.criarTela();
    }

    private void Sair_Click(object sender, RoutedEventArgs e) => MostrarLogin();
}
