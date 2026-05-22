 using DeliverySystem.Domain.Interfaces;

namespace DeliverySystem.Domain.Entities;

public sealed class Prato : Produto, IPreparacao
{
    private string _tipoCozinha = string.Empty;
    private string _tamanho = string.Empty;
    private string _ingredientes = string.Empty;

    public string TipoCozinha
    {
        get => _tipoCozinha;
        private set => _tipoCozinha = value;
    }

    public string Tamanho
    {
        get => _tamanho;
        private set => _tamanho = value;
    }

    public string Ingredientes
    {
        get => _ingredientes;
        private set => _ingredientes = value;
    }

    private Prato()
    {
    }

    public Prato(
        int restauranteId,
        string nome,
        string descricao,
        decimal preco,
        int tempoPreparoMin,
        string? imagemPath,
        string tipoCozinha,
        string tamanho,
        string ingredientes)
        : base(restauranteId, nome, descricao, preco, tempoPreparoMin, imagemPath)
    {
        TipoCozinha = tipoCozinha;
        Tamanho = tamanho;
        Ingredientes = ingredientes;
    }

    public override string ExibirInformacoes()
    {
        return $"{base.ExibirInformacoes()} | {TipoCozinha} | {Tamanho}";
    }

    public string Preparar()
    {
        return $"Preparando {Nome} ({TempoPreparoMin} min)";
    }
}
