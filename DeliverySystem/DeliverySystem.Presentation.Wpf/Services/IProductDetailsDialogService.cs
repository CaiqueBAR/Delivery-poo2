using DeliverySystem.Presentation.Wpf.ViewModels;

namespace DeliverySystem.Presentation.Wpf.Services;

public interface IProductDetailsDialogService
{
    void Show(ProductCardViewModel product);
}

