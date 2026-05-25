using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DeliverySystem.Domain.Entities;
using DeliverySystem.Presentation.Wpf.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DeliverySystem.Presentation.Wpf.ViewModels;

public partial class HomePageViewModel : ObservableObject
{
    private readonly ScopedRunner _runner;
    private readonly IProductDetailsDialogService _dialog;

    public ObservableCollection<CategoryChipViewModel> Categorias { get; } = new();
    public ObservableCollection<BannerViewModel> Banners { get; } = new();
    public ObservableCollection<ProductCardViewModel> Ofertas { get; } = new();

    [ObservableProperty]
    private string _promoBarText = "Você tem cupons! Aproveite seus descontos";

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string? _errorMessage;

    public HomePageViewModel(ScopedRunner runner, IProductDetailsDialogService dialog)
    {
        _runner = runner;
        _dialog = dialog;

        Categorias.Add(new CategoryChipViewModel("Padarias"));
        Categorias.Add(new CategoryChipViewModel("Doces"));
        Categorias.Add(new CategoryChipViewModel("Lanches"));
        Categorias.Add(new CategoryChipViewModel("Promoções"));
        Categorias.Add(new CategoryChipViewModel("Brasileira"));
        Categorias.Add(new CategoryChipViewModel("Salgados"));
        Categorias.Add(new CategoryChipViewModel("Marmita"));
        Categorias.Add(new CategoryChipViewModel("Saudável"));
        Categorias.Add(new CategoryChipViewModel("Pastel"));
        Categorias.Add(new CategoryChipViewModel("Açaí"));

        Banners.Add(new BannerViewModel("Desconto no café da manhã"));
        Banners.Add(new BannerViewModel("Top restaurantes com desconto"));
        Banners.Add(new BannerViewModel("Festival do hambúrguer"));
        Banners.Add(new BannerViewModel("Cupom R$30"));
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
                var produtosService = sp.GetRequiredService<Application.Services.ProdutoAppService>();
                var restaurantesService = sp.GetRequiredService<Application.Services.RestauranteAppService>();

                var produtos = await produtosService.SearchByNomeAsync(string.Empty);
                var restaurantes = await restaurantesService.ListAsync();
                var restaurantesMap = restaurantes.ToDictionary(r => r.Id, r => r.Nome);

                Ofertas.Clear();
                foreach (var p in produtos.OrderByDescending(p => p.Preco).Take(12))
                {
                    Ofertas.Add(new ProductCardViewModel(
                        p.Id,
                        p.Nome,
                        p.Descricao,
                        p.ImagemPath,
                        p.Preco,
                        p is Prato ? "Prato" : p is Bebida ? "Bebida" : "Produto",
                        restaurantesMap.TryGetValue(p.RestauranteId, out var nome) ? nome : null,
                        4.7m,
                        35,
                        7.90m));
                }
            });
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
    private void OpenProduct(ProductCardViewModel? product)
    {
        if (product is null) return;
        _dialog.Show(product);
    }

    private void LoadMock()
    {
        Ofertas.Clear();
        Ofertas.Add(new ProductCardViewModel(1, "Sub Frango Defumado", "Sanduíche com cream cheese", null, 27.50m, "Prato", "Subway Aeroporto", 4.7m, 35, 7.90m));
        Ofertas.Add(new ProductCardViewModel(2, "Burger Smash", "Burger duplo com fritas", null, 32.90m, "Prato", "Burger House", 4.8m, 29, 0m));
        Ofertas.Add(new ProductCardViewModel(3, "Coca-Cola 350ml", "Bebida gelada", null, 7.00m, "Bebida", "Burger House", 4.8m, 29, 0m));
        Ofertas.Add(new ProductCardViewModel(4, "Temaki Salmão", "Temaki premium 180g", null, 26.50m, "Prato", "Sushi Now", 4.6m, 45, 9.90m));
    }
}

public sealed record CategoryChipViewModel(string Nome, string? IconPath = null);
public sealed record BannerViewModel(string Titulo, string? ImagePath = null);
