using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DeliverySystem.Presentation.Wpf.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DeliverySystem.Presentation.Wpf.ViewModels;

public partial class RestaurantsPageViewModel : ObservableObject
{
    private readonly ScopedRunner _runner;

    public ObservableCollection<string> Filtros { get; } = new();
    public ObservableCollection<RestaurantCardViewModel> Restaurantes { get; } = new();

    [ObservableProperty]
    private string _titulo = "Lanches";

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string? _errorMessage;

    public RestaurantsPageViewModel(ScopedRunner runner)
    {
        _runner = runner;

        Filtros.Add("Ordenar");
        Filtros.Add("Entrega Grátis");
        Filtros.Add("Vale-refeição");
        Filtros.Add("Distância");
        Filtros.Add("Entrega Parceira");
        Filtros.Add("Pra retirar");
        Filtros.Add("Filtros");
        Filtros.Add("Limpar");
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
                var service = sp.GetRequiredService<Application.Services.RestauranteAppService>();
                var restaurantes = await service.ListAsync();

                Restaurantes.Clear();
                foreach (var r in restaurantes.OrderBy(r => r.Nome))
                {
                    Restaurantes.Add(new RestaurantCardViewModel(
                        r.Id,
                        r.Nome,
                        r.Categoria,
                        4.7m,
                        "Hoje, entre 19h-1h",
                        "•",
                        "R$ 0,00",
                        null));
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

    private void LoadMock()
    {
        Restaurantes.Clear();
        Restaurantes.Add(new RestaurantCardViewModel(1, "Subway Aeroporto", "Lanches", 4.7m, "29-39 min", "•", "R$ 18,99", null));
        Restaurantes.Add(new RestaurantCardViewModel(2, "Vix Burger - Hamburgueria", "Lanches", 4.9m, "Hoje, entre 19h-1h", "•", "R$ 12,99", null));
        Restaurantes.Add(new RestaurantCardViewModel(3, "Empório Don Avelino", "Lanches", 4.6m, "41-56 min", "•", "R$ 19,98", null));
        Restaurantes.Add(new RestaurantCardViewModel(4, "Thai Toneto Lanches", "Lanches", 4.4m, "48-63 min", "•", "R$ 22,99", null));
        Restaurantes.Add(new RestaurantCardViewModel(5, "Hot Dog do Marcos", "Lanches", 4.8m, "Hoje, entre 18h-2h", "•", "R$ 22,00", null));
        Restaurantes.Add(new RestaurantCardViewModel(6, "Mr Burger", "Lanches", 4.5m, "Hoje, entre 19h-1h", "•", "R$ 12,99", null));
    }
}

public sealed record RestaurantCardViewModel(
    int Id,
    string Nome,
    string Categoria,
    decimal Avaliacao,
    string Tempo,
    string Separador,
    string Entrega,
    string? LogoPath);
