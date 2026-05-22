using DeliverySystem.Domain.Entities;

namespace DeliverySystem.Application.Interfaces;

public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default);
}
