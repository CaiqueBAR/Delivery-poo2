namespace DeliverySystem.Application.DTOs;

public sealed record ClienteCreateDto(
    int UsuarioId,
    string Nome,
    string Telefone,
    string EnderecoPadrao);

public sealed record ClienteUpdateDto(
    string Nome,
    string Telefone,
    string EnderecoPadrao);
