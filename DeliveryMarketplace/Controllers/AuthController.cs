using DeliveryMarketplace.Data;
using DeliveryMarketplace.Models;
using DeliveryMarketplace.Repositories;

namespace DeliveryMarketplace.Controllers;

/// <summary>Fase 3: login e cadastro de clientes.</summary>
public class AuthController
{
    private readonly UsuarioRepository _usuarios = new();

    public Usuario Entrar(string email, string senha)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
        {
            throw new RegraException("Informe e-mail e senha.");
        }

        var usuario = _usuarios.BuscarPorEmail(email.Trim().ToLower());
        if (usuario is null || usuario.SenhaHash != Seguranca.HashSenha(senha))
        {
            throw new RegraException("E-mail ou senha incorretos.");
        }

        if (!usuario.Ativo)
        {
            throw new RegraException("Esta conta foi desativada pelo administrador.");
        }

        Sessao.Entrar(usuario);
        return usuario;
    }

    public void CadastrarCliente(string nome, string email, string senha, string telefone, string cpf, string endereco)
    {
        if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(endereco))
        {
            throw new RegraException("Preencha nome e endereço.");
        }

        email = (email ?? "").Trim().ToLower();
        if (!email.Contains('@') || !email.Contains('.'))
        {
            throw new RegraException("E-mail inválido.");
        }

        if ((senha ?? "").Length < 6)
        {
            throw new RegraException("A senha deve ter pelo menos 6 caracteres.");
        }

        if (_usuarios.EmailExiste(email))
        {
            throw new RegraException("Já existe uma conta com este e-mail.");
        }

        _usuarios.Adicionar(new Usuario
        {
            Nome = nome.Trim(),
            Email = email,
            SenhaHash = Seguranca.HashSenha(senha!),
            Telefone = telefone?.Trim() ?? "",
            Cpf = cpf?.Trim(),
            Endereco = endereco.Trim(),
            Perfil = Perfil.Cliente
        });
    }

    public void Sair() => Sessao.Sair();
}
