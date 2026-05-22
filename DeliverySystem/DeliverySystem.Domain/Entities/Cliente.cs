namespace DeliverySystem.Domain.Entities;

public sealed class Cliente : EntityBase
{
    private int _usuarioId;
    private string _nome = string.Empty;
    private string _telefone = string.Empty;
    private string _enderecoPadrao = string.Empty;

    public int UsuarioId
    {
        get => _usuarioId;
        private set => _usuarioId = value;
    }

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

    public string EnderecoPadrao
    {
        get => _enderecoPadrao;
        private set => _enderecoPadrao = value;
    }

    private Cliente()
    {
    }

    public Cliente(int usuarioId, string nome, string telefone, string enderecoPadrao)
    {
        UsuarioId = usuarioId;
        Nome = nome;
        Telefone = telefone;
        EnderecoPadrao = enderecoPadrao;
    }

    public void Atualizar(string nome, string telefone, string enderecoPadrao)
    {
        Nome = nome;
        Telefone = telefone;
        EnderecoPadrao = enderecoPadrao;
    }
}
