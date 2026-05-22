using DeliverySystem.Domain.Enums;
using DeliverySystem.Domain.Interfaces;

namespace DeliverySystem.Domain.Entities;

public sealed class Pedido : EntityBase, IPedidoSubject
{
    private DateTime _dataPedido;
    private PedidoStatus _status;
    private decimal _valorTotal;
    private string _clienteNome = string.Empty;
    private string _enderecoEntrega = string.Empty;
    private List<ItemDePedido> _itens = new();
    private readonly List<IPedidoObserver> _observers = new();

    public DateTime DataPedido
    {
        get => _dataPedido;
        private set => _dataPedido = value;
    }

    public PedidoStatus Status
    {
        get => _status;
        private set => _status = value;
    }

    public decimal ValorTotal
    {
        get => _valorTotal;
        private set => _valorTotal = value;
    }

    public string ClienteNome
    {
        get => _clienteNome;
        private set => _clienteNome = value;
    }

    public string EnderecoEntrega
    {
        get => _enderecoEntrega;
        private set => _enderecoEntrega = value;
    }

    public List<ItemDePedido> Itens
    {
        get => _itens;
        private set => _itens = value;
    }

    private Pedido()
    {
    }

    public Pedido(string clienteNome, string enderecoEntrega)
    {
        DataPedido = DateTime.UtcNow;
        Status = PedidoStatus.Recebido;
        ClienteNome = clienteNome;
        EnderecoEntrega = enderecoEntrega;
    }

    public void AdicionarItem(Produto produto, int quantidade)
    {
        var existente = _itens.FirstOrDefault(i => i.Produto.Id == produto.Id);
        if (existente is null)
        {
            _itens.Add(new ItemDePedido(produto, quantidade));
        }
        else
        {
            existente.AlterarQuantidade(existente.Quantidade + quantidade);
        }

        CalcularTotal();
    }

    public void RemoverItem(int produtoId)
    {
        var item = _itens.FirstOrDefault(i => i.Produto.Id == produtoId);
        if (item is null) return;

        _itens.Remove(item);
        CalcularTotal();
    }

    public void AlterarQuantidade(int produtoId, int quantidade)
    {
        var item = _itens.FirstOrDefault(i => i.Produto.Id == produtoId);
        if (item is null) return;

        item.AlterarQuantidade(quantidade);
        CalcularTotal();
    }

    public decimal CalcularTotal()
    {
        foreach (var item in _itens)
        {
            item.CalcularSubtotal();
        }

        ValorTotal = _itens.Sum(i => i.Subtotal);
        return ValorTotal;
    }

    public decimal CalcularTotalComStrategies(IEnumerable<IPedidoPricingStrategy> strategies)
    {
        var valor = CalcularTotal();
        foreach (var strategy in strategies)
        {
            valor = strategy.Apply(this, valor);
        }

        ValorTotal = valor;
        return ValorTotal;
    }

    public void AlterarStatus(PedidoStatus novoStatus)
    {
        Status = novoStatus;
        NotifyStatusChanged();
    }

    public void FinalizarPedido()
    {
        AlterarStatus(PedidoStatus.Entregue);
    }

    public void Subscribe(IPedidoObserver observer)
    {
        if (_observers.Contains(observer)) return;
        _observers.Add(observer);
    }

    public void Unsubscribe(IPedidoObserver observer)
    {
        _observers.Remove(observer);
    }

    private void NotifyStatusChanged()
    {
        foreach (var observer in _observers.ToArray())
        {
            observer.OnStatusChanged(Id, Status);
        }
    }
}
