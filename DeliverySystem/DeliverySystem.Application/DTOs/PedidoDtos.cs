namespace DeliverySystem.Application.DTOs;

public sealed record PedidoCreateDto(
    string ClienteNome,
    string EnderecoEntrega,
    string? CupomCodigo);

public sealed record PedidoItemAddDto(
    int ProdutoId,
    int Quantidade);
