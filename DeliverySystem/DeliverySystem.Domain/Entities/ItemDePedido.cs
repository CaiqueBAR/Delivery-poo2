namespace DeliverySystem.Domain.Entities;

public sealed class ItemDePedido : EntityBase
{
    private int _quantidade;
    private decimal _subtotal;
    private Produto _produto = null!;

    public int Quantidade
    {
        get => _quantidade;
        private set => _quantidade = value;
    }

    public decimal Subtotal
    {
        get => _subtotal;
        private set => _subtotal = value;
    }

    public Produto Produto
    {
        get => _produto;
        private set => _produto = value;
    }

    private ItemDePedido()
    {
    }

    public ItemDePedido(Produto produto, int quantidade)
    {
        Produto = produto;
        Quantidade = quantidade;
        CalcularSubtotal();
    }

    public void AlterarQuantidade(int quantidade)
    {
        Quantidade = quantidade;
        CalcularSubtotal();
    }

    public void CalcularSubtotal()
    {
        Subtotal = Produto.Preco * Quantidade;
    }
}
