using DeliverySystem.Domain.Entities;

namespace DeliverySystem.Application.Interfaces;

public interface IRestauranteRepository : IRepository<Restaurante>
{
    Task<IReadOnlyList<Restaurante>> SearchByNomeAsync(string termo, CancellationToken ct = default);
}
