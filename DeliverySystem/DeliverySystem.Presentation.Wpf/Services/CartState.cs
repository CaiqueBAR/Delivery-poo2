using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DeliverySystem.Presentation.Wpf.Services;

public sealed partial class CartState : ObservableObject
{
    public ObservableCollection<CartItem> Items { get; } = new();

    [ObservableProperty]
    private int _itemsCount;

    [ObservableProperty]
    private decimal _total;

    public CartState()
    {
        Items.CollectionChanged += OnItemsChanged;
    }

    public void AddOrIncrease(int productId, string nome, decimal unitPrice, string? imagePath, int quantity)
    {
        if (quantity <= 0) return;

        var existing = Items.FirstOrDefault(i => i.ProductId == productId);
        if (existing is not null)
        {
            existing.Quantity += quantity;
            return;
        }

        var item = new CartItem(productId, nome, unitPrice, imagePath, quantity);
        Items.Add(item);
        item.PropertyChanged += OnItemPropertyChanged;

        Recalculate();
    }

    public void Decrease(int productId, int quantity)
    {
        if (quantity <= 0) return;

        var item = Items.FirstOrDefault(i => i.ProductId == productId);
        if (item is null) return;

        item.Quantity -= quantity;
        if (item.Quantity <= 0)
        {
            Remove(productId);
        }
    }

    public void Remove(int productId)
    {
        var item = Items.FirstOrDefault(i => i.ProductId == productId);
        if (item is null) return;

        item.PropertyChanged -= OnItemPropertyChanged;
        Items.Remove(item);
        Recalculate();
    }

    public void Clear()
    {
        foreach (var item in Items)
        {
            item.PropertyChanged -= OnItemPropertyChanged;
        }

        Items.Clear();
        Recalculate();
    }

    private void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems is not null)
        {
            foreach (CartItem item in e.OldItems)
            {
                item.PropertyChanged -= OnItemPropertyChanged;
            }
        }

        if (e.NewItems is not null)
        {
            foreach (CartItem item in e.NewItems)
            {
                item.PropertyChanged += OnItemPropertyChanged;
            }
        }

        Recalculate();
    }

    private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CartItem.Quantity))
        {
            Recalculate();
        }
    }

    private void Recalculate()
    {
        ItemsCount = Items.Sum(i => i.Quantity);
        Total = Items.Sum(i => i.UnitPrice * i.Quantity);
    }
}

public sealed partial class CartItem : ObservableObject
{
    public int ProductId { get; }
    public string Nome { get; }
    public decimal UnitPrice { get; }
    public string? ImagePath { get; }

    [ObservableProperty]
    private int _quantity;

    public decimal Subtotal => UnitPrice * Quantity;

    public CartItem(int productId, string nome, decimal unitPrice, string? imagePath, int quantity)
    {
        ProductId = productId;
        Nome = nome;
        UnitPrice = unitPrice;
        ImagePath = imagePath;
        _quantity = Math.Max(1, quantity);
    }

    partial void OnQuantityChanged(int value)
    {
        OnPropertyChanged(nameof(Subtotal));
    }
}
