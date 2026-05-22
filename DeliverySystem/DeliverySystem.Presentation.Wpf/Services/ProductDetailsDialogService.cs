using DeliverySystem.Presentation.Wpf.Views;
using DeliverySystem.Presentation.Wpf.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace DeliverySystem.Presentation.Wpf.Services;

public sealed class ProductDetailsDialogService : IProductDetailsDialogService
{
    private readonly IServiceProvider _services;

    public ProductDetailsDialogService(IServiceProvider services)
    {
        _services = services;
    }

    public void Show(ProductCardViewModel product)
    {
        var vm = ActivatorUtilities.CreateInstance<ProductDetailsDialogViewModel>(_services, product);
        var window = new ProductDetailsWindow(vm)
        {
            Owner = App.Current?.MainWindow
        };

        window.ShowDialog();
    }
}

