namespace DeliverySystem.Domain.Entities;

public sealed class Entregador : EntityBase
{
    private string _nome = string.Empty;
    private string _telefone = string.Empty;
    private string _veiculo = string.Empty;
    private bool _ativo;

    public string Nome
    {
        get => _nome;
        private set => _nome = value;
    }

    public string Telefone
    {
        get => _telefone;
        private set => _telefone = value;
    }

    public string Veiculo
    {
        get => _veiculo;
        private set => _veiculo = value;
    }

    public bool Ativo
    {
        get => _ativo;
        private set => _ativo = value;
    }

    private Entregador()
    {
    }

    public Entregador(string nome, string telefone, string veiculo)
    {
        Nome = nome;
        Telefone = telefone;
        Veiculo = veiculo;
        Ativo = true;
    }

    public void Desativar()
    {
        Ativo = false;
    }
}
