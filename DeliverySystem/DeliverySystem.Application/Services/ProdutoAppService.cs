using DeliverySystem.Application.DTOs;
using DeliverySystem.Application.Interfaces;
using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Patterns;

namespace DeliverySystem.Application.Services;

public sealed class ProdutoAppService
{
    private readonly IProdutoRepository _repo;
    private readonly IUnitOfWork _uow;

    public ProdutoAppService(IProdutoRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<int> CreatePratoAsync(PratoCreateDto dto, CancellationToken ct = default)
    {
        var factory = new PratoFactory(
            dto.RestauranteId,
            dto.Nome,
            dto.Descricao,
            dto.Preco,
            dto.TempoPreparoMin,
            dto.ImagemPath,
            dto.TipoCozinha,
            dto.Tamanho,
            dto.Ingredientes);

        var prato = factory.Criar();
        await _repo.AddAsync(prato, ct);
        await _uow.SaveChangesAsync(ct);
        return prato.Id;
    }

    public async Task<int> CreateBebidaAsync(BebidaCreateDto dto, CancellationToken ct = default)
    {
        var factory = new BebidaFactory(
            dto.RestauranteId,
            dto.Nome,
            dto.Descricao,
            dto.Preco,
            dto.TempoPreparoMin,
            dto.ImagemPath,
            dto.VolumeMl,
            dto.Gelada,
            dto.Tipo);

        var bebida = factory.Criar();
        await _repo.AddAsync(bebida, ct);
        await _uow.SaveChangesAsync(ct);
        return bebida.Id;
    }

    public Task<IReadOnlyList<Produto>> ListByRestauranteAsync(int restauranteId, CancellationToken ct = default)
    {
        return _repo.ListByRestauranteAsync(restauranteId, ct);
    }

    public Task<IReadOnlyList<Produto>> SearchByNomeAsync(string termo, CancellationToken ct = default)
    {
        return _repo.SearchByNomeAsync(termo, ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var produto = await _repo.GetByIdAsync(id, ct) ?? throw new InvalidOperationException("Produto não encontrado.");
        _repo.Remove(produto);
        await _uow.SaveChangesAsync(ct);
    }
}
