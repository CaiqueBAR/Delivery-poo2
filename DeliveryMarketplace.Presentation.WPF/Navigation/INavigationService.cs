using DeliveryMarketplace.Presentation.WPF.ViewModels;

namespace DeliveryMarketplace.Presentation.WPF.Navigation;

/// <summary>
/// Serviço de navegação entre páginas. Os ViewModels navegam por tipo,
/// sem conhecer as Views (que são escolhidas pelos DataTemplates).
/// </summary>
public interface INavigationService
{
    ViewModelBase? CurrentViewModel { get; }

    event EventHandler? CurrentViewModelChanged;

    Task NavigateToAsync<TViewModel>() where TViewModel : ViewModelBase;

    Task NavigateToAsync(Type viewModelType);
}
