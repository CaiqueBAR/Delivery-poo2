using System.Windows;
using System.Windows.Controls;
using DeliveryMarketplace.Controllers;
using DeliveryMarketplace.Models;

namespace DeliveryMarketplace.Views.Entregadores;

public partial class EntregadorView : UserControl
{
    private readonly EntregadorController _controller = new();

    public EntregadorView()
    {
        InitializeComponent();
        Componentes.Executar(Carregar);
    }

    private void Carregar()
    {
        var eu = _controller.EuMesmo();
        TxtNome.Text = eu.Nome;
        TxtSituacao.Text = $"{(eu.Disponivel ? "Disponível para entregas" : "Indisponível")} · {eu.Veiculo}";
        BtnDisponivel.Content = eu.Disponivel ? "Ficar indisponível" : "Ficar disponível";

        Componentes.MostrarIndicadores(Indicadores, _controller.Ganhos());

        var minhas = _controller.MinhasEntregas();
        GradeMinhas.ItemsSource = minhas.Where(p => p.Status != StatusPedido.Entregue && p.Status != StatusPedido.Cancelado).ToList();
        GradeHistorico.ItemsSource = minhas.Where(p => p.Status == StatusPedido.Entregue).ToList();
        GradeDisponiveis.ItemsSource = _controller.EntregasDisponiveis();
    }

    private void Atualizar_Click(object sender, RoutedEventArgs e) => Componentes.Executar(Carregar);

    private void Disponivel_Click(object sender, RoutedEventArgs e) => Acao(_controller.FicarDisponivelOuNao);

    private void Aceitar_Click(object sender, RoutedEventArgs e)
    {
        var pedido = Componentes.Item<Pedido>(sender);
        Acao(() => _controller.AceitarEntrega(pedido.Id));
    }

    private void Retirada_Click(object sender, RoutedEventArgs e)
    {
        var pedido = Componentes.Item<Pedido>(sender);
        Acao(() => _controller.ConfirmarRetirada(pedido.Id));
    }

    private void Entrega_Click(object sender, RoutedEventArgs e)
    {
        var pedido = Componentes.Item<Pedido>(sender);
        Acao(() => _controller.ConfirmarEntrega(pedido.Id), "Entrega concluída!");
    }

    private void Acao(Action acao, string? sucesso = null)
    {
        if (Componentes.Executar(acao, sucesso))
        {
            Carregar();
        }
    }
}
