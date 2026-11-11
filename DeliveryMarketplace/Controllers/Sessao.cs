using DeliveryMarketplace.Models;

namespace DeliveryMarketplace.Controllers;

/// <summary>Guarda quem está logado no momento.</summary>
public static class Sessao
{
    public static Usuario? UsuarioLogado { get; private set; }

    public static void Entrar(Usuario usuario) => UsuarioLogado = usuario;

    public static void Sair() => UsuarioLogado = null;

    /// <summary>Usuário logado (lança erro se ninguém estiver logado).</summary>
    public static Usuario Usuario => UsuarioLogado ?? throw new RegraException("Faça login novamente.");
}

/// <summary>Erro de regra de negócio. A mensagem é exibida para o usuário.</summary>
public class RegraException : Exception
{
    public RegraException(string mensagem) : base(mensagem)
    {
    }
}
