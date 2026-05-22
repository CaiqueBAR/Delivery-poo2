using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DeliverySystem.Presentation.Wpf.Services;

namespace DeliverySystem.Presentation.Wpf.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly HomePageViewModel _home;
    private readonly RestaurantsPageViewModel _restaurants;
    private readonly ProductsPageViewModel _products;
    private readonly CartState _cart;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private object? _currentPage;

    public ObservableCollection<NavTabViewModel> Tabs { get; } = new();

    public CartState Cart => _cart;

    public MainViewModel(
        HomePageViewModel home,
        RestaurantsPageViewModel restaurants,
        ProductsPageViewModel products,
        CartState cart)
    {
        _home = home;
        _restaurants = restaurants;
        _products = products;
        _cart = cart;

        Tabs.Add(new NavTabViewModel("Inicio", "Início", isSelected: true));
        Tabs.Add(new NavTabViewModel("Restaurantes", "Restaurantes"));
        Tabs.Add(new NavTabViewModel("Mercados", "Mercados"));
        Tabs.Add(new NavTabViewModel("Bebidas", "Bebidas"));
        Tabs.Add(new NavTabViewModel("Farmacias", "Farmácias"));
        Tabs.Add(new NavTabViewModel("Pets", "Pets"));
        Tabs.Add(new NavTabViewModel("Shopping", "Shopping"));

        CurrentPage = _home;
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        await NavigateAsync("Inicio");
    }

    [RelayCommand]
    private async Task SelectTabAsync(NavTabViewModel? tab)
    {
        if (tab is null) return;
        await NavigateAsync(tab.Key);
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        var term = SearchText.Trim();
        if (string.IsNullOrWhiteSpace(term)) return;

        await NavigateAsync("Produtos", term);
    }

    private async Task NavigateAsync(string key, string? searchTerm = null)
    {
        var hasTab = Tabs.Any(t => t.Key == key);
        if (hasTab)
        {
            foreach (var t in Tabs)
            {
                t.IsSelected = t.Key == key;
            }
        }

        switch (key)
        {
            case "Inicio":
                CurrentPage = _home;
                await _home.LoadAsync();
                break;

            case "Produtos":
                CurrentPage = _products;
                await _products.LoadAsync();
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    _products.Busca = searchTerm;
                    await _products.SearchAsync();
                }
                break;

            case "Restaurantes":
                CurrentPage = _restaurants;
                await _restaurants.LoadAsync();
                break;

            default:
                CurrentPage = new PlaceholderPageViewModel(
                    key,
                    "Seção preparada. Podemos ativar esta tela conforme o escopo do cliente.");
                break;
        }
    }
}
