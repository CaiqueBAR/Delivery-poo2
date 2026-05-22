using DeliverySystem.Application.DTOs;
using DeliverySystem.Application.Interfaces;
using DeliverySystem.Domain.Entities;

namespace DeliverySystem.Application.Services;

public sealed class AuthAppService
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _jwt;
    private readonly IUnitOfWork _uow;

    public AuthAppService(IUsuarioRepository usuarios, IPasswordHasher hasher, IJwtTokenService jwt, IUnitOfWork uow)
    {
        _usuarios = usuarios;
        _hasher = hasher;
        _jwt = jwt;
        _uow = uow;
    }

    public async Task RegisterAsync(RegisterDto dto, CancellationToken ct = default)
    {
        var existing = await _usuarios.GetByEmailAsync(dto.Email, ct);
        if (existing is not null) throw new InvalidOperationException("Email já cadastrado.");

        var hash = _hasher.Hash(dto.Password);
        var user = new Usuario(dto.Email, hash, dto.Role);
        await _usuarios.AddAsync(user, ct);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task<LoginResultDto> LoginAsync(LoginDto dto, CancellationToken ct = default)
    {
        var user = await _usuarios.GetByEmailAsync(dto.Email, ct);
        if (user is null) throw new InvalidOperationException("Credenciais inválidas.");
        if (!user.Ativo) throw new InvalidOperationException("Usuário desativado.");

        var ok = _hasher.Verify(dto.Password, user.PasswordHash);
        if (!ok) throw new InvalidOperationException("Credenciais inválidas.");

        var token = _jwt.GenerateToken(user);
        return new LoginResultDto(token);
    }
}
