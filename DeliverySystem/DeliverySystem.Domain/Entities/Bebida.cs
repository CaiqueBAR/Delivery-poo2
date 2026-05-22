namespace DeliverySystem.Domain.Entities;

public sealed class Bebida : Produto
{
    private int _volumeMl;
    private bool _gelada;
    private string _tipo = string.Empty;

    public int VolumeMl
    {
        get => _volumeMl;
        private set => _volumeMl = value;
    }

    public bool Gelada
    {
        get => _gelada;
        private set => _gelada = value;
    }

    public string Tipo
    {
        get => _tipo;
        private set => _tipo = value;
    }

    private Bebida()
    {
    }

    public Bebida(
        int restauranteId,
        string nome,
        string descricao,
        decimal preco,
        int tempoPreparoMin,
        string? imagemPath,
        int volumeMl,
        bool gelada,
        string tipo)
        : base(restauranteId, nome, descricao, preco, tempoPreparoMin, imagemPath)
    {
        VolumeMl = volumeMl;
        Gelada = gelada;
        Tipo = tipo;
    }

    public override string ExibirInformacoes()
    {
        var temp = Gelada ? "Gelada" : "Natural";
        return $"{base.ExibirInformacoes()} | {VolumeMl}ml | {temp} | {Tipo}";
    }
}
