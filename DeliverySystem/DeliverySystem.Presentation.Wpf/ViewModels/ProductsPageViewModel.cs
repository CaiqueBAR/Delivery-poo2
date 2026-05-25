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
    private int _searchVersion;

    public ObservableCollection<RestaurantOptionViewModel> Restaurantes { get; } = new();
    public ObservableCollection<ProductCardViewModel> Itens { get; } = new();

    [ObservableProperty]
    private RestaurantOptionViewModel? _restauranteSelecionado;

    [ObservableProperty]
    private string _busca = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string? _errorMessage;

    public ProductsPageViewModel(ScopedRunner runner, IProductDetailsDialogService dialog)
    {
        _runner = runner;
        _dialog = dialog;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        IsLoading = true;
        ErrorMessage = null;
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
        }
        catch
        {
            LoadMock();
            ErrorMessage = "Modo demonstração: não foi possível carregar do banco.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task SearchAsync()
    {
        var version = Interlocked.Increment(ref _searchVersion);
        var termo = Busca?.Trim() ?? string.Empty;
        var selectedId = RestauranteSelecionado?.Id;
        IReadOnlyList<Produto> produtos = Array.Empty<Produto>();

        IsLoading = true;
        ErrorMessage = null;
        try
        {
            await _runner.RunAsync(async sp =>
            {
                var produtosService = sp.GetRequiredService<Application.Services.ProdutoAppService>();

                if (selectedId is int restauranteId)
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
        }
        catch
        {
            LoadMockItems(termo);
            ErrorMessage = "Modo demonstração: busca carregada localmente.";
            return;
        }
        finally
        {
            if (version == _searchVersion)
            {
                IsLoading = false;
            }
        }

        if (version != _searchVersion) return;

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

    private void LoadMock()
    {
        _restauranteNomes.Clear();
        _restauranteNomes[1] = "Burger House";
        _restauranteNomes[2] = "Sushi Now";

        Restaurantes.Clear();
        Restaurantes.Add(new RestaurantOptionViewModel(null, "Todos os restaurantes"));
        Restaurantes.Add(new RestaurantOptionViewModel(1, "Burger House"));
        Restaurantes.Add(new RestaurantOptionViewModel(2, "Sushi Now"));
        RestauranteSelecionado ??= Restaurantes.FirstOrDefault();

        LoadMockItems(Busca);
    }

    private void LoadMockItems(string? termo)
    {
        var t = termo?.Trim() ?? string.Empty;
        var all = new List<ProductCardViewModel>
        {
            new(1, "Burger Smash", "Burger duplo com fritas", null, 32.90m, "Prato", "Burger House", 4.8m, 29, 0m),
            new(2, "Combo Frango", "Sanduíche + refri", null, 27.50m, "Prato", "Burger House", 4.7m, 35, 7.90m),
            new(3, "Temaki Salmão", "Temaki premium 180g", null, 26.50m, "Prato", "Sushi Now", 4.6m, 45, 9.90m),
            new(4, "Coca-Cola 350ml", "Bebida gelada", null, 7.00m, "Bebida", "Burger House", 4.8m, 29, 0m)
        };

        var filtered = string.IsNullOrWhiteSpace(t)
            ? all
            : all.Where(p => p.Nome.Contains(t, StringComparison.OrdinalIgnoreCase)).ToList();

        Itens.Clear();
        foreach (var p in filtered)
        {
            Itens.Add(p);
        }
    }
}

public sealed record RestaurantOptionViewModel(int? Id, string Nome);
