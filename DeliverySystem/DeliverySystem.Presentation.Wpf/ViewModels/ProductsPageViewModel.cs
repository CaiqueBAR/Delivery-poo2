using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DeliverySystem.Domain.Entities;
using DeliverySystem.Presentation.Wpf.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DeliverySystem.Presentation.Wpf.ViewModels;

public partial class ProductsPageViewModel : ObservableObject
{
    private readonly ScopedRunner _runner;
    private readonly IProductDetailsDialogService _dialog;
    private readonly Dictionary<int, string> _restauranteNomes = new();

    public ObservableCollection<RestaurantOptionViewModel> Restaurantes { get; } = new();
    public ObservableCollection<ProductCardViewModel> Itens { get; } = new();

    [ObservableProperty]
    private RestaurantOptionViewModel? _restauranteSelecionado;

    [ObservableProperty]
    private string _busca = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    public ProductsPageViewModel(ScopedRunner runner, IProductDetailsDialogService dialog)
    {
        _runner = runner;
        _dialog = dialog;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            await _runner.RunAsync(async sp =>
            {
                var restaurantesService = sp.GetRequiredService<Application.Services.RestauranteAppService>();
                var restaurantes = await restaurantesService.ListAsync();

                _restauranteNomes.Clear();
                foreach (var r in restaurantes)
                {
                    _restauranteNomes[r.Id] = r.Nome;
                }

                Restaurantes.Clear();
                Restaurantes.Add(new RestaurantOptionViewModel(null, "Todos os restaurantes"));
                foreach (var r in restaurantes.OrderBy(r => r.Nome))
                {
                    Restaurantes.Add(new RestaurantOptionViewModel(r.Id, r.Nome));
                }
            });

            RestauranteSelecionado ??= Restaurantes.FirstOrDefault();
            await SearchAsync();
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task SearchAsync()
    {
        var termo = Busca?.Trim() ?? string.Empty;
        IReadOnlyList<Produto> produtos = Array.Empty<Produto>();

        await _runner.RunAsync(async sp =>
        {
            var produtosService = sp.GetRequiredService<Application.Services.ProdutoAppService>();

            if (RestauranteSelecionado?.Id is int restauranteId)
            {
                var baseList = await produtosService.ListByRestauranteAsync(restauranteId);
                produtos = string.IsNullOrWhiteSpace(termo)
                    ? baseList
                    : baseList.Where(p => p.Nome.Contains(termo, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            else
            {
                produtos = await produtosService.SearchByNomeAsync(termo);
            }
        });

        Itens.Clear();
        foreach (var p in produtos.OrderBy(p => p.Nome))
        {
            Itens.Add(new ProductCardViewModel(
                p.Id,
                p.Nome,
                p.Descricao,
                p.ImagemPath,
                p.Preco,
                p is Prato ? "Prato" : p is Bebida ? "Bebida" : "Produto",
                _restauranteNomes.TryGetValue(p.RestauranteId, out var nome) ? nome : null,
                4.7m,
                35,
                7.90m));
        }
    }

    [RelayCommand]
    private void OpenProduct(ProductCardViewModel? card)
    {
        if (card is null) return;
        _dialog.Show(card);
    }
}

public sealed record RestaurantOptionViewModel(int? Id, string Nome);
