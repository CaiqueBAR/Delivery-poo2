using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Interfaces;

namespace DeliverySystem.Domain.Patterns;

public sealed class PratoFactory : IProdutoFactory<Prato>
{
    private readonly int _restauranteId;
    private readonly string _nome;
    private readonly string _descricao;
    private readonly decimal _preco;
    private readonly int _tempoPreparoMin;
    private readonly string? _imagemPath;
    private readonly string _tipoCozinha;
    private readonly string _tamanho;
    private readonly string _ingredientes;

    public PratoFactory(
        int restauranteId,
        string nome,
        string descricao,
        decimal preco,
        int tempoPreparoMin,
        string? imagemPath,
        string tipoCozinha,
        string tamanho,
        string ingredientes)
    {
        _restauranteId = restauranteId;
        _nome = nome;
        _descricao = descricao;
        _preco = preco;
        _tempoPreparoMin = tempoPreparoMin;
        _imagemPath = imagemPath;
        _tipoCozinha = tipoCozinha;
        _tamanho = tamanho;
        _ingredientes = ingredientes;
    }

    public Prato Criar()
    {
        return new Prato(
            _restauranteId,
            _nome,
            _descricao,
            _preco,
            _tempoPreparoMin,
            _imagemPath,
            _tipoCozinha,
            _tamanho,
            _ingredientes);
    }
}
