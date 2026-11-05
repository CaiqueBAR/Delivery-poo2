using DeliveryMarketplace.Presentation.WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace DeliveryMarketplace.Presentation.WPF.Navigation;

/// <summary>
/// Resolve o ViewModel de destino pelo container de DI e o define como página atual.
/// </summary>
public sealed class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ViewModelBase? CurrentViewModel { get; private set; }

    public event EventHandler? CurrentViewModelChanged;

    public Task NavigateToAsync<TViewModel>() where TViewModel : ViewModelBase
        => NavigateToAsync(typeof(TViewModel));

    public async Task NavigateToAsync(Type viewModelType)
    {
        if (!typeof(ViewModelBase).IsAssignableFrom(viewModelType))
        {
            throw new ArgumentException($"{viewModelType.Name} não é um ViewModel de página.", nameof(viewModelType));
        }

        var viewModel = (ViewModelBase)_serviceProvider.GetRequiredService(viewModelType);

        CurrentViewModel = viewModel;
        CurrentViewModelChanged?.Invoke(this, EventArgs.Empty);

        if (viewModel is INavigationAware aware)
        {
            await aware.OnNavigatedToAsync();
        }
    }
}
