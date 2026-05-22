namespace DeliverySystem.Application.DTOs;

public sealed record RestauranteCreateDto(
    string Nome,
    string Endereco,
    string Telefone,
    string Categoria);

public sealed record RestauranteUpdateDto(
    string Nome,
    string Endereco,
    string Telefone,
    string Categoria);
