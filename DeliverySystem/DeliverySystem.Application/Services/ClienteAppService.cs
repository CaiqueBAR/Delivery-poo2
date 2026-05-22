using DeliverySystem.Application.DTOs;
using DeliverySystem.Application.Interfaces;
using DeliverySystem.Domain.Entities;

namespace DeliverySystem.Application.Services;

public sealed class ClienteAppService
{
    private readonly IClienteRepository _repo;
    private readonly IUnitOfWork _uow;

    public ClienteAppService(IClienteRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<int> CreateAsync(ClienteCreateDto dto, CancellationToken ct = default)
    {
        var entity = new Cliente(dto.UsuarioId, dto.Nome, dto.Telefone, dto.EnderecoPadrao);
        await _repo.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);
        return entity.Id;
    }

    public Task<IReadOnlyList<Cliente>> ListAsync(CancellationToken ct = default)
    {
        return _repo.ListAsync(ct);
    }

    public async Task UpdateAsync(int id, ClienteUpdateDto dto, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct) ?? throw new InvalidOperationException("Cliente não encontrado.");
        entity.Atualizar(dto.Nome, dto.Telefone, dto.EnderecoPadrao);
        _repo.Update(entity);
        await _uow.SaveChangesAsync(ct);
    }
}
