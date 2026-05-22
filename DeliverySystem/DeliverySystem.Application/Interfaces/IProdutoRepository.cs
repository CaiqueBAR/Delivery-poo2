using DeliverySystem.Domain.Entities;

namespace DeliverySystem.Application.Interfaces;

public interface IProdutoRepository : IRepository<Produto>
{
    Task<IReadOnlyList<Produto>> ListByRestauranteAsync(int restauranteId, CancellationToken ct = default);
    Task<IReadOnlyList<Produto>> SearchByNomeAsync(string termo, CancellationToken ct = default);
}
