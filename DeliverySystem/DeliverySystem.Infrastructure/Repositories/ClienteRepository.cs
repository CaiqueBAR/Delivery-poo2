using DeliverySystem.Application.Interfaces;
using DeliverySystem.Domain.Entities;
using DeliverySystem.Infrastructure.Database;

namespace DeliverySystem.Infrastructure.Repositories;

public sealed class ClienteRepository : EfRepository<Cliente>, IClienteRepository
{
    public ClienteRepository(DeliveryDbContext db) : base(db)
    {
    }
}
