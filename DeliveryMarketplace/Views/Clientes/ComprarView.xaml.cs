using System.Windows;
using System.Windows.Controls;
using DeliveryMarketplace.Controllers;
using DeliveryMarketplace.Models;

namespace DeliveryMarketplace.Views.Clientes;

public partial class ComprarView : UserControl
{
    private readonly ClienteController _controller = new();
    private readonly List<ItemCarrinho> _carrinho = [];
    private Restaurante? _restauranteDoCarrinho;

    public ComprarView()
    {
        InitializeComponent();
        LstRestaurantes.ItemsSource = _controller.ListarRestaurantes();
        CmbPagamento.ItemsSource = Enum.GetValues<MetodoPagamento>();
        CmbPagamento.SelectedIndex = 0;
        AtualizarCarrinho();
    }

    private Restaurante? Selecionado => LstRestaurantes.SelectedItem as Restaurante;

    private void Restaurante_Changed(object sender, SelectionChangedEventArgs e)
    {
        if (Selecionado is null)
        {
            return;
        }

        TxtCardapio.Text = Selecionado.Aberto ? $"Cardápio · {Selecionado.Nome}" : $"{Selecionado.Nome} (fechado agora)";
        GradeCardapio.ItemsSource = _controller.Cardapio(Selecionado.Id);
    }

    private void Adicionar_Click(object sender, RoutedEventArgs e)
    {
        var produto = Componentes.Item<Produto>(sender);
        if (Componentes.Executar(() => _controller.AdicionarAoCarrinho(_carrinho, produto)))
        {
            _restauranteDoCarrinho = Selecionado;
            AtualizarCarrinho();
        }
    }

    private void Esvaziar_Click(object sender, RoutedEventArgs e)
    {
        _carrinho.Clear();
        _restauranteDoCarrinho = null;
        AtualizarCarrinho();
    }

    private void Pagamento_Changed(object sender, SelectionChangedEventArgs e)
    {
        var metodo = (MetodoPagamento)CmbPagamento.SelectedItem;
        PainelCartao.Visibility = metodo is MetodoPagamento.CartaoCredito or MetodoPagamento.CartaoDebito
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    private void Finalizar_Click(object sender, RoutedEventArgs e)
    {
        Pedido? pedido = null;
        var ok = Componentes.Executar(() =>
        {
            if (_restauranteDoCarrinho is null)
            {
                throw new RegraException("Seu carrinho está vazio.");
            }

            pedido = _controller.FazerPedido(_restauranteDoCarrinho, _carrinho, (MetodoPagamento)CmbPagamento.SelectedItem,
                TxtCartao.Text, TxtObservacao.Text);
        });

        if (ok && pedido is not null)
        {
            MessageBox.Show($"Pedido {pedido.Codigo} enviado!\nTotal: {pedido.Total:C}\nPagamento: {pedido.Pagamento?.Status}",
                "Pedido realizado", MessageBoxButton.OK, MessageBoxImage.Information);
            Esvaziar_Click(sender, e);
            TxtCartao.Text = TxtObservacao.Text = "";
        }
    }

    private void AtualizarCarrinho()
    {
        LstCarrinho.ItemsSource = null;
        LstCarrinho.ItemsSource = _carrinho;

        var subtotal = _carrinho.Sum(i => i.Subtotal);
        var taxa = _restauranteDoCarrinho?.TaxaEntrega ?? 0;
        TxtSubtotal.Text = $"Subtotal: {subtotal:C}";
        TxtTaxa.Text = $"Entrega: {taxa:C}";
        TxtTotal.Text = $"Total: {subtotal + taxa:C}";
    }
}
