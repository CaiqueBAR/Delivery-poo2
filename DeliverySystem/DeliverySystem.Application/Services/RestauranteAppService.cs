using DeliverySystem.Application.DTOs;
using DeliverySystem.Application.Interfaces;
using DeliverySystem.Domain.Entities;

namespace DeliverySystem.Application.Services;

public sealed class RestauranteAppService
{
    private readonly IRestauranteRepository _repo;
    private readonly IUnitOfWork _uow;

    public RestauranteAppService(IRestauranteRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<int> CreateAsync(RestauranteCreateDto dto, CancellationToken ct = default)
    {
        var restaurante = new Restaurante(dto.Nome, dto.Endereco, dto.Telefone, dto.Categoria);
        await _repo.AddAsync(restaurante, ct);
        await _uow.SaveChangesAsync(ct);
        return restaurante.Id;
    }

    public Task<IReadOnlyList<Restaurante>> ListAsync(CancellationToken ct = default)
    {
        return _repo.ListAsync(ct);
    }

    public Task<IReadOnlyList<Restaurante>> SearchByNomeAsync(string termo, CancellationToken ct = default)
    {
        return _repo.SearchByNomeAsync(termo, ct);
    }

    public async Task UpdateAsync(int id, RestauranteUpdateDto dto, CancellationToken ct = default)
    {
        var restaurante = await _repo.GetByIdAsync(id, ct) ?? throw new InvalidOperationException("Restaurante não encontrado.");
        restaurante.AtualizarDados(dto.Nome, dto.Endereco, dto.Telefone, dto.Categoria);
        _repo.Update(restaurante);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var restaurante = await _repo.GetByIdAsync(id, ct) ?? throw new InvalidOperationException("Restaurante não encontrado.");
        _repo.Remove(restaurante);
        await _uow.SaveChangesAsync(ct);
    }
}
