namespace DeliverySystem.Domain.Entities;

public sealed class AvaliacaoProduto : EntityBase
{
    private int _produtoId;
    private int _clienteId;
    private int _nota;
    private string _comentario = string.Empty;
    private DateTime _dataCriacao;

    public int ProdutoId
    {
        get => _produtoId;
        private set => _produtoId = value;
    }

    public int ClienteId
    {
        get => _clienteId;
        private set => _clienteId = value;
    }

    public int Nota
    {
        get => _nota;
        private set => _nota = value;
    }

    public string Comentario
    {
        get => _comentario;
        private set => _comentario = value;
    }

    public DateTime DataCriacao
    {
        get => _dataCriacao;
        private set => _dataCriacao = value;
    }

    private AvaliacaoProduto()
    {
    }

    public AvaliacaoProduto(int produtoId, int clienteId, int nota, string comentario)
    {
        ProdutoId = produtoId;
        ClienteId = clienteId;
        Nota = nota;
        Comentario = comentario;
        DataCriacao = DateTime.UtcNow;
    }
}
