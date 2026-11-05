using CommunityToolkit.Mvvm.Input;
using DeliveryMarketplace.Presentation.WPF.Navigation;
using DeliveryMarketplace.Presentation.WPF.ViewModels.Items;

namespace DeliveryMarketplace.Presentation.WPF.ViewModels;

/// <summary>
/// Página inicial: apresentação do marketplace, perfis e roteiro das fases.
/// </summary>
public partial class HomeViewModel : ViewModelBase
{
    private readonly INavigationService _navigation;

    public HomeViewModel(INavigationService navigation)
    {
        _navigation = navigation;
    }

    public override string Title => "Início";
    public override string Subtitle => "Visão geral do marketplace e do andamento do projeto";

    public IReadOnlyList<PerfilCard> Perfis { get; } =
    [
        new("Cliente", "", "Consulta restaurantes, monta o carrinho, paga e acompanha o pedido.", "Fase 7"),
        new("Restaurante", "", "Gerencia cardápio, categorias e o fluxo de preparo dos pedidos.", "Fase 5"),
        new("Entregador", "", "Aceita entregas, confirma retirada e entrega, acompanha ganhos.", "Fase 6"),
        new("Administrador", "", "Visão global: cadastros, pedidos, pagamentos, taxas e relatórios.", "Fase 4")
    ];

    public IReadOnlyList<FaseItem> Fases { get; } =
    [
        new(1, "Estrutura inicial WPF", true),
        new(2, "Diagrama de classes UML e modelagem", false),
        new(3, "Autenticação e perfis", false),
        new(4, "Administração", false),
        new(5, "Restaurante", false),
        new(6, "Entregador", false),
        new(7, "Cliente", false),
        new(8, "Pagamentos simulados", false),
        new(9, "Relatórios e finalização", false)
    ];

    [RelayCommand]
    private Task OpenStatusAsync() => _navigation.NavigateToAsync<SystemStatusViewModel>();
}
