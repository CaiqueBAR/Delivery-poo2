namespace DeliverySystem.Application.DTOs;

public sealed record PratoCreateDto(
    int RestauranteId,
    string Nome,
    string Descricao,
    decimal Preco,
    int TempoPreparoMin,
    string? ImagemPath,
    string TipoCozinha,
    string Tamanho,
    string Ingredientes);

public sealed record BebidaCreateDto(
    int RestauranteId,
    string Nome,
    string Descricao,
    decimal Preco,
    int TempoPreparoMin,
    string? ImagemPath,
    int VolumeMl,
    bool Gelada,
    string Tipo);
