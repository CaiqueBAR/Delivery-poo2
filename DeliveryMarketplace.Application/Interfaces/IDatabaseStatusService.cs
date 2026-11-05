using DeliveryMarketplace.Application.DTOs;

namespace DeliveryMarketplace.Application.Interfaces;

/// <summary>
/// Contrato para consultar o estado da configuração do banco de dados.
/// A implementação fica na camada Infrastructure.
/// </summary>
public interface IDatabaseStatusService
{
    Task<DatabaseStatusDto> GetStatusAsync(CancellationToken cancellationToken = default);
}
