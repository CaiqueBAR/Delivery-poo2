using DeliverySystem.Domain.Enums;

namespace DeliverySystem.Domain.Entities;

public sealed class Cupom : EntityBase
{
    private string _codigo = string.Empty;
    private CupomTipo _tipo;
    private decimal _valor;
    private bool _ativo;

    public string Codigo
    {
        get => _codigo;
        private set => _codigo = value;
    }

    public CupomTipo Tipo
    {
        get => _tipo;
        private set => _tipo = value;
    }

    public decimal Valor
    {
        get => _valor;
        private set => _valor = value;
    }

    public bool Ativo
    {
        get => _ativo;
        private set => _ativo = value;
    }

    private Cupom()
    {
    }

    public Cupom(string codigo, CupomTipo tipo, decimal valor, bool ativo)
    {
        Codigo = codigo;
        Tipo = tipo;
        Valor = valor;
        Ativo = ativo;
    }

    public void Ativar()
    {
        Ativo = true;
    }

    public void Desativar()
    {
        Ativo = false;
    }
}
