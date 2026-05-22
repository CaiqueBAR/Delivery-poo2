using DeliverySystem.Domain.Enums;

namespace DeliverySystem.Domain.Entities;

public sealed class Usuario : EntityBase
{
    private string _email = string.Empty;
    private string _passwordHash = string.Empty;
    private UserRole _role;
    private bool _ativo;

    public string Email
    {
        get => _email;
        private set => _email = value;
    }

    public string PasswordHash
    {
        get => _passwordHash;
        private set => _passwordHash = value;
    }

    public UserRole Role
    {
        get => _role;
        private set => _role = value;
    }

    public bool Ativo
    {
        get => _ativo;
        private set => _ativo = value;
    }

    private Usuario()
    {
    }

    public Usuario(string email, string passwordHash, UserRole role)
    {
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        Ativo = true;
    }

    public void Desativar()
    {
        Ativo = false;
    }
}
