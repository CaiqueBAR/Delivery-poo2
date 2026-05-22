using DeliverySystem.Application.Interfaces;
using DeliverySystem.Domain.Entities;
using DeliverySystem.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace DeliverySystem.Infrastructure.Repositories;

public sealed class UsuarioRepository : EfRepository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(DeliveryDbContext db) : base(db)
    {
    }

    public Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        email = email.Trim().ToLowerInvariant();
        return Db.Usuarios.FirstOrDefaultAsync(u => u.Email.ToLower() == email, ct);
    }
}
