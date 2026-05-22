using CommunityToolkit.Mvvm.ComponentModel;

namespace DeliverySystem.Presentation.Wpf.Services;

public sealed partial class CartState : ObservableObject
{
    [ObservableProperty]
    private int _itemsCount;

    [ObservableProperty]
    private decimal _total;

    public void Add(decimal unitPrice, int quantity)
    {
        ItemsCount += quantity;
        Total += unitPrice * quantity;
    }
}

