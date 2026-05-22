using DeliverySystem.Domain.Entities;

namespace DeliverySystem.Application.Interfaces;

public interface ICupomRepository : IRepository<Cupom>
{
    Task<Cupom?> GetByCodigoAsync(string codigo, CancellationToken ct = default);
}
