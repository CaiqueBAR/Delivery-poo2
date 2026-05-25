using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DeliverySystem.Presentation.Wpf.Services;

namespace DeliverySystem.Presentation.Wpf.ViewModels;

public partial class ProductDetailsDialogViewModel : ObservableObject
{
    private readonly CartState _cart;

    public event Action? CloseRequested;

    public int Id { get; }
    public string Nome { get; }
    public string Descricao { get; }
    public decimal Preco { get; }
    public string Tipo { get; }
    public string? RestauranteNome { get; }
    public decimal? RestauranteAvaliacao { get; }

    public string PrecoTexto => $"R$ {Preco:0.00}";

    public ObservableCollection<string> Imagens { get; } = new();

    [ObservableProperty]
    private string? _imagemSelecionada;

    [ObservableProperty]
    private int _quantidade = 1;

    public ProductDetailsDialogViewModel(CartState cart, ProductCardViewModel product)
    {
        _cart = cart;

        Id = product.Id;
        Nome = product.Nome;
        Descricao = product.Descricao;
        Preco = product.Preco;
        Tipo = product.Tipo;
        RestauranteNome = product.RestauranteNome;
        RestauranteAvaliacao = product.RestauranteAvaliacao;

        foreach (var img in SplitImages(product.ImagemPath))
        {
            Imagens.Add(img);
        }

        ImagemSelecionada = Imagens.FirstOrDefault();
    }

    [RelayCommand]
    private void Close()
    {
        CloseRequested?.Invoke();
    }

    [RelayCommand]
    private void SelectThumbnail(string? path)
    {
        ImagemSelecionada = path;
    }

    [RelayCommand]
    private void IncreaseQuantity()
    {
        Quantidade += 1;
    }

    [RelayCommand]
    private void DecreaseQuantity()
    {
        Quantidade = Math.Max(1, Quantidade - 1);
    }

    [RelayCommand]
    private void AddToCart()
    {
        _cart.AddOrIncrease(Id, Nome, Preco, ImagemSelecionada ?? Imagens.FirstOrDefault(), Quantidade);
        CloseRequested?.Invoke();
    }

    private static IReadOnlyList<string> SplitImages(string? imagemPath)
    {
        if (string.IsNullOrWhiteSpace(imagemPath)) return Array.Empty<string>();

        return imagemPath
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToArray();
    }
}
