using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DeliverySystem.Presentation.Wpf.Services;

namespace DeliverySystem.Presentation.Wpf.ViewModels;

public sealed partial class CartPageViewModel : ObservableObject
{
    private readonly CartState _cart;

    [ObservableProperty]
    private string? _statusMessage;

    public ObservableCollection<CartItem> Items => _cart.Items;
    public CartState Cart => _cart;

    public CartPageViewModel(CartState cart)
    {
        _cart = cart;
    }

    [RelayCommand]
    private void Increase(CartItem? item)
    {
        if (item is null) return;
        _cart.AddOrIncrease(item.ProductId, item.Nome, item.UnitPrice, item.ImagePath, 1);
    }

    [RelayCommand]
    private void Decrease(CartItem? item)
    {
        if (item is null) return;
        _cart.Decrease(item.ProductId, 1);
    }

    [RelayCommand]
    private void Remove(CartItem? item)
    {
        if (item is null) return;
        _cart.Remove(item.ProductId);
    }

    [RelayCommand]
    private void Clear()
    {
        _cart.Clear();
        StatusMessage = null;
    }

    [RelayCommand]
    private void Checkout()
    {
        if (_cart.ItemsCount <= 0)
        {
            StatusMessage = "Seu carrinho está vazio.";
            return;
        }

        StatusMessage = "Checkout preparado. Podemos integrar pagamento/entrega conforme o escopo.";
    }
}
