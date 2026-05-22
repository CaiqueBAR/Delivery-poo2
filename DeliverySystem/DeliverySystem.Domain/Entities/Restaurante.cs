namespace DeliverySystem.Domain.Entities;

public sealed class Restaurante : EntityBase
{
    private string _nome = string.Empty;
    private string _endereco = string.Empty;
    private string _telefone = string.Empty;
    private string _categoria = string.Empty;
    private readonly List<Produto> _listaProdutos = new();

    public string Nome
    {
        get => _nome;
        private set => _nome = value;
    }

    public string Endereco
    {
        get => _endereco;
        private set => _endereco = value;
    }

    public string Telefone
    {
        get => _telefone;
        private set => _telefone = value;
    }

    public string Categoria
    {
        get => _categoria;
        private set => _categoria = value;
    }

    public IReadOnlyCollection<Produto> ListaProdutos => _listaProdutos;

    private Restaurante()
    {
    }

    public Restaurante(string nome, string endereco, string telefone, string categoria)
    {
        Nome = nome;
        Endereco = endereco;
        Telefone = telefone;
        Categoria = categoria;
    }

    public void AtualizarDados(string nome, string endereco, string telefone, string categoria)
    {
        Nome = nome;
        Endereco = endereco;
        Telefone = telefone;
        Categoria = categoria;
    }

    public void AdicionarProduto(Produto produto)
    {
        _listaProdutos.Add(produto);
    }

    public void RemoverProduto(Produto produto)
    {
        _listaProdutos.Remove(produto);
    }

    public IReadOnlyCollection<Produto> ListarProdutos()
    {
        return _listaProdutos;
    }
}
