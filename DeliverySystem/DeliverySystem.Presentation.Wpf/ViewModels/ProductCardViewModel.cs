namespace DeliverySystem.Presentation.Wpf.ViewModels;

public sealed record ProductCardViewModel(
    int Id,
    string Nome,
    string Descricao,
    string? ImagemPath,
    decimal Preco,
    string Tipo,
    string? RestauranteNome = null,
    decimal? RestauranteAvaliacao = null,
    int? TempoMin = null,
    decimal? EntregaPreco = null);

