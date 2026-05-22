namespace DeliverySystem.Domain.Entities;

public abstract class Produto : EntityBase
{
    private int _restauranteId;
    private string _nome = string.Empty;
    private string _descricao = string.Empty;
    private decimal _preco;
    private int _tempoPreparoMin;
    private string? _imagemPath;

    public int RestauranteId
    {
        get => _restauranteId;
        private set => _restauranteId = value;
    }

    public string Nome
    {
        get => _nome;
        protected set => _nome = value;
    }

    public string Descricao
    {
        get => _descricao;
        protected set => _descricao = value;
    }

    public decimal Preco
    {
        get => _preco;
        protected set => _preco = value;
    }

    public int TempoPreparoMin
    {
        get => _tempoPreparoMin;
        protected set => _tempoPreparoMin = value;
    }

    public string? ImagemPath
    {
        get => _imagemPath;
        protected set => _imagemPath = value;
    }

    protected Produto()
    {
    }

    protected Produto(int restauranteId, string nome, string descricao, decimal preco, int tempoPreparoMin, string? imagemPath)
    {
        RestauranteId = restauranteId;
        Nome = nome;
        Descricao = descricao;
        Preco = preco;
        TempoPreparoMin = tempoPreparoMin;
        ImagemPath = imagemPath;
    }

    public virtual string ExibirInformacoes()
    {
        return $"{Nome} - R$ {Preco:0.00} ({TempoPreparoMin} min)";
    }
}
