using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DeliveryMarketplace.Presentation.WPF.Navigation;

namespace DeliveryMarketplace.Presentation.WPF.ViewModels;

/// <summary>
/// ViewModel da janela principal: menu lateral e página atual.
/// </summary>
public partial class MainWindowViewModel : ObservableObject
{
    private readonly INavigationService _navigation;

    [ObservableProperty]
    private ViewModelBase? _currentViewModel;

    public MainWindowViewModel(INavigationService navigation)
    {
        _navigation = navigation;
        _navigation.CurrentViewModelChanged += OnCurrentViewModelChanged;

        MenuItems =
        [
            new NavigationItem("Início", "", typeof(HomeViewModel)),
            new NavigationItem("Status do sistema", "", typeof(SystemStatusViewModel))
        ];
    }

    public ObservableCollection<NavigationItem> MenuItems { get; }

    public string VersionLabel { get; } =
        $"v{typeof(MainWindowViewModel).Assembly.GetName().Version?.ToString(3)} · Fase 1";

    public Task InitializeAsync() => _navigation.NavigateToAsync<HomeViewModel>();

    [RelayCommand]
    private Task NavigateAsync(NavigationItem? item)
        => item is null ? Task.CompletedTask : _navigation.NavigateToAsync(item.ViewModelType);

    private void OnCurrentViewModelChanged(object? sender, EventArgs e)
    {
        CurrentViewModel = _navigation.CurrentViewModel;

        // Mantém o menu sincronizado mesmo quando a navegação parte de dentro de uma página.
        foreach (var item in MenuItems)
        {
            item.IsSelected = item.ViewModelType == CurrentViewModel?.GetType();
        }
    }
}
