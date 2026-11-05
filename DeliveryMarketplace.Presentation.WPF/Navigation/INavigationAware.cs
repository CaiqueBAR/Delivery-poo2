namespace DeliveryMarketplace.Presentation.WPF.Navigation;

/// <summary>
/// Implementado por ViewModels que precisam carregar dados ao serem exibidos.
/// </summary>
public interface INavigationAware
{
    Task OnNavigatedToAsync();
}
