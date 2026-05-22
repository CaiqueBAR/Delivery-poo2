using DeliverySystem.Application.DTOs;
using DeliverySystem.Application.Interfaces;
using DeliverySystem.Application.Services.Observers;
using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Enums;
using DeliverySystem.Domain.Patterns;

namespace DeliverySystem.Application.Services;

public sealed class PedidoAppService
{
    private readonly IPedidoRepository _pedidoRepo;
    private readonly IProdutoRepository _produtoRepo;
    private readonly ICupomRepository _cupomRepo;
    private readonly IUnitOfWork _uow;
    private readonly ClientePedidoObserver _observer;

    public PedidoAppService(
        IPedidoRepository pedidoRepo,
        IProdutoRepository produtoRepo,
        ICupomRepository cupomRepo,
        IUnitOfWork uow,
        ClientePedidoObserver observer)
    {
        _pedidoRepo = pedidoRepo;
        _produtoRepo = produtoRepo;
        _cupomRepo = cupomRepo;
        _uow = uow;
        _observer = observer;
    }

    public async Task<int> CreateAsync(PedidoCreateDto dto, CancellationToken ct = default)
    {
        var pedido = new Pedido(dto.ClienteNome, dto.EnderecoEntrega);
        pedido.Subscribe(_observer);

        await _pedidoRepo.AddAsync(pedido, ct);
        await _uow.SaveChangesAsync(ct);
        return pedido.Id;
    }

    public async Task AddItemAsync(int pedidoId, PedidoItemAddDto dto, CancellationToken ct = default)
    {
        var pedido = await _pedidoRepo.GetByIdAsync(pedidoId, ct) ?? throw new InvalidOperationException("Pedido não encontrado.");
        var produto = await _produtoRepo.GetByIdAsync(dto.ProdutoId, ct) ?? throw new InvalidOperationException("Produto não encontrado.");

        pedido.AdicionarItem(produto, dto.Quantidade);
        _pedidoRepo.Update(pedido);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task AlterarQuantidadeAsync(int pedidoId, int produtoId, int quantidade, CancellationToken ct = default)
    {
        var pedido = await _pedidoRepo.GetByIdAsync(pedidoId, ct) ?? throw new InvalidOperationException("Pedido não encontrado.");
        pedido.AlterarQuantidade(produtoId, quantidade);
        _pedidoRepo.Update(pedido);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task RemoverItemAsync(int pedidoId, int produtoId, CancellationToken ct = default)
    {
        var pedido = await _pedidoRepo.GetByIdAsync(pedidoId, ct) ?? throw new InvalidOperationException("Pedido não encontrado.");
        pedido.RemoverItem(produtoId);
        _pedidoRepo.Update(pedido);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task<decimal> RecalcularTotalAsync(int pedidoId, string? cupomCodigo, CancellationToken ct = default)
    {
        var pedido = await _pedidoRepo.GetByIdAsync(pedidoId, ct) ?? throw new InvalidOperationException("Pedido não encontrado.");
        var strategies = await BuildStrategiesAsync(cupomCodigo, ct);
        var total = pedido.CalcularTotalComStrategies(strategies);
        _pedidoRepo.Update(pedido);
        await _uow.SaveChangesAsync(ct);
        return total;
    }

    public async Task AlterarStatusAsync(int pedidoId, PedidoStatus status, CancellationToken ct = default)
    {
        var pedido = await _pedidoRepo.GetByIdAsync(pedidoId, ct) ?? throw new InvalidOperationException("Pedido não encontrado.");
        pedido.AlterarStatus(status);
        _pedidoRepo.Update(pedido);
        await _uow.SaveChangesAsync(ct);
    }

    public Task<IReadOnlyList<Pedido>> ListByStatusAsync(PedidoStatus status, CancellationToken ct = default)
    {
        return _pedidoRepo.ListByStatusAsync(status, ct);
    }

    private async Task<IReadOnlyList<Domain.Interfaces.IPedidoPricingStrategy>> BuildStrategiesAsync(string? cupomCodigo, CancellationToken ct)
    {
        var list = new List<Domain.Interfaces.IPedidoPricingStrategy>();

        var cupom = await GetCupomAsync(cupomCodigo, ct);
        var cupomFreteGratis = cupom?.Tipo == CupomTipo.FreteGratis;

        if (!cupomFreteGratis)
        {
            list.Add(new TaxaEntregaStrategy(7.90m));
        }

        list.Add(new PromocaoHorarioStrategy(TimeSpan.FromHours(1), TimeSpan.FromHours(5), 10m));

        if (cupom is not null)
        {
            list.Add(new CupomStrategy(cupom));
        }

        return list;
    }

    private async Task<Cupom?> GetCupomAsync(string? codigo, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(codigo)) return null;

        var db = await _cupomRepo.GetByCodigoAsync(codigo, ct);
        if (db is not null) return db;

        return codigo.Trim().ToUpperInvariant() switch
        {
            "CUPOM10" => new Cupom("CUPOM10", CupomTipo.Percentual, 10m, true),
            "DESCONTO20" => new Cupom("DESCONTO20", CupomTipo.Percentual, 20m, true),
            "FRETEGRATIS" => new Cupom("FRETEGRATIS", CupomTipo.FreteGratis, 0m, true),
            _ => null
        };
    }
}
