using DeliverySystem.Domain.Entities;

namespace DeliverySystem.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(Usuario usuario);
}
