using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DeliverySystem.Domain.Enums;
using DeliverySystem.Presentation.Wpf.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DeliverySystem.Presentation.Wpf.ViewModels;

public partial class DashboardPageViewModel : ObservableObject
{
    private readonly ScopedRunner _runner;

    public ObservableCollection<DashboardCardViewModel> Cards { get; } = new();
    public ObservableCollection<RestauranteListItemViewModel> Restaurantes { get; } = new();
    public ObservableCollection<ProdutoListItemViewModel> Produtos { get; } = new();
    public ObservableCollection<PedidoListItemViewModel> Pedidos { get; } = new();

    [ObservableProperty]
    private string _statusResumo = "Pronto";

    public DashboardPageViewModel(
        ScopedRunner runner)
    {
        _runner = runner;

        Cards.Add(new DashboardCardViewModel("Pedidos Hoje", "0", "Pedidos totais do dia"));
        Cards.Add(new DashboardCardViewModel("Em Andamento", "0", "Pedidos em preparo e rota"));
        Cards.Add(new DashboardCardViewModel("Vendas do Dia", "R$ 0,00", "Receita consolidada"));
        Cards.Add(new DashboardCardViewModel("Cupom Destaque", "CUPOM10", "Promoção ativa"));
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        try
        {
            await _runner.RunAsync(async sp =>
            {
                var restaurantesService = sp.GetRequiredService<Application.Services.RestauranteAppService>();
                var produtosService = sp.GetRequiredService<Application.Services.ProdutoAppService>();
                var pedidosService = sp.GetRequiredService<Application.Services.PedidoAppService>();
                var relatoriosService = sp.GetRequiredService<Application.Services.RelatorioAppService>();

                var restaurantes = await restaurantesService.ListAsync();
                var produtos = await produtosService.SearchByNomeAsync(string.Empty);
                var pedidosRecebidos = await pedidosService.ListByStatusAsync(PedidoStatus.Recebido);
                var emPreparo = await pedidosService.ListByStatusAsync(PedidoStatus.EmPreparo);
                var saiuEntrega = await pedidosService.ListByStatusAsync(PedidoStatus.SaiuParaEntrega);
                var faturamento = await relatoriosService.ValorVendidoNoDiaAsync(DateOnly.FromDateTime(DateTime.UtcNow));

                Restaurantes.Clear();
                foreach (var restaurante in restaurantes.Take(8))
                {
                    Restaurantes.Add(new RestauranteListItemViewModel(restaurante.Nome, restaurante.Categoria, restaurante.Endereco));
                }

                Produtos.Clear();
                foreach (var produto in produtos.Take(12))
                {
                    Produtos.Add(new ProdutoListItemViewModel(produto.Id, produto.Nome, produto.Descricao, produto.ImagemPath, $"R$ {produto.Preco:0.00}"));
                }

                Pedidos.Clear();
                foreach (var pedido in pedidosRecebidos.Concat(emPreparo).Concat(saiuEntrega).Take(10))
                {
                    Pedidos.Add(new PedidoListItemViewModel(pedido.Id, $"#{pedido.Id}", pedido.ClienteNome, pedido.Status.ToString(), $"R$ {pedido.ValorTotal:0.00}"));
                }

                UpdateCards(
                    pedidosRecebidos.Count + emPreparo.Count + saiuEntrega.Count,
                    emPreparo.Count + saiuEntrega.Count,
                    faturamento,
                    restaurantes.Count);
            });

            StatusResumo = "Carregado do banco";
        }
        catch
        {
            LoadMock();
            StatusResumo = "Modo demonstração";
        }
    }

    private void UpdateCards(int pedidosHoje, int emAndamento, decimal vendasDia, int restaurantesAtivos)
    {
        Cards.Clear();
        Cards.Add(new DashboardCardViewModel("Pedidos Hoje", pedidosHoje.ToString(), "Pedidos confirmados"));
        Cards.Add(new DashboardCardViewModel("Em Andamento", emAndamento.ToString(), "Preparo ou entrega"));
        Cards.Add(new DashboardCardViewModel("Vendas do Dia", $"R$ {vendasDia:0.00}", "Receita operacional"));
        Cards.Add(new DashboardCardViewModel("Restaurantes", restaurantesAtivos.ToString(), "Lojas ativas"));
    }

    private void LoadMock()
    {
        UpdateCards(42, 11, 1823.40m, 8);

        Restaurantes.Clear();
        Restaurantes.Add(new RestauranteListItemViewModel("Burger House", "Hamburguer", "Rua A, 123"));
        Restaurantes.Add(new RestauranteListItemViewModel("Sushi Now", "Japonesa", "Av. B, 456"));
        Restaurantes.Add(new RestauranteListItemViewModel("Pasta Prime", "Italiana", "Rua C, 89"));

        Produtos.Clear();
        Produtos.Add(new ProdutoListItemViewModel(1, "Combo Smash", "Burger duplo com fritas", null, "R$ 32,90"));
        Produtos.Add(new ProdutoListItemViewModel(2, "Temaki Salmão", "Temaki premium 180g", null, "R$ 26,50"));
        Produtos.Add(new ProdutoListItemViewModel(3, "Coca-Cola 350ml", "Bebida gelada", null, "R$ 7,00"));

        Pedidos.Clear();
        Pedidos.Add(new PedidoListItemViewModel(1201, "#1201", "Ana Souza", "Recebido", "R$ 54,90"));
        Pedidos.Add(new PedidoListItemViewModel(1202, "#1202", "Bruno Lima", "EmPreparo", "R$ 39,90"));
        Pedidos.Add(new PedidoListItemViewModel(1203, "#1203", "Carla Dias", "SaiuParaEntrega", "R$ 62,40"));
    }
}

public sealed record DashboardCardViewModel(string Titulo, string Valor, string Descricao);
public sealed record RestauranteListItemViewModel(string Nome, string Categoria, string Endereco);
public sealed record ProdutoListItemViewModel(int Id, string Nome, string Descricao, string? ImagemPath, string Preco);
public sealed record PedidoListItemViewModel(int Id, string Numero, string Cliente, string Status, string Valor);
