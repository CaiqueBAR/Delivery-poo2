using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Interfaces;

namespace DeliverySystem.Domain.Patterns;

public sealed class BebidaFactory : IProdutoFactory<Bebida>
{
    private readonly int _restauranteId;
    private readonly string _nome;
    private readonly string _descricao;
    private readonly decimal _preco;
    private readonly int _tempoPreparoMin;
    private readonly string? _imagemPath;
    private readonly int _volumeMl;
    private readonly bool _gelada;
    private readonly string _tipo;

    public BebidaFactory(
        int restauranteId,
        string nome,
        string descricao,
        decimal preco,
        int tempoPreparoMin,
        string? imagemPath,
        int volumeMl,
        bool gelada,
        string tipo)
    {
        _restauranteId = restauranteId;
        _nome = nome;
        _descricao = descricao;
        _preco = preco;
        _tempoPreparoMin = tempoPreparoMin;
        _imagemPath = imagemPath;
        _volumeMl = volumeMl;
        _gelada = gelada;
        _tipo = tipo;
    }

    public Bebida Criar()
    {
        return new Bebida(
            _restauranteId,
            _nome,
            _descricao,
            _preco,
            _tempoPreparoMin,
            _imagemPath,
            _volumeMl,
            _gelada,
            _tipo);
    }
}
