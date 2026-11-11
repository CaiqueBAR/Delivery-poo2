using DeliveryMarketplace.Data;
using DeliveryMarketplace.Models;
using DeliveryMarketplace.Repositories;

namespace DeliveryMarketplace.Controllers;

/// <summary>Fase 4: administração da plataforma.</summary>
public class AdminController
{
    private readonly UsuarioRepository _usuarios = new();
    private readonly RestauranteRepository _restaurantes = new();
    private readonly PedidoRepository _pedidos = new();

    public List<Usuario> ListarUsuarios() =>
        _usuarios.ListarTodos().OrderBy(u => u.Perfil).ThenBy(u => u.Nome).ToList();

    public void AtivarOuDesativarUsuario(int usuarioId)
    {
        if (usuarioId == Sessao.Usuario.Id)
        {
            throw new RegraException("Você não pode desativar a própria conta.");
        }

        var usuario = _usuarios.BuscarPorId(usuarioId) ?? throw new RegraException("Usuário não encontrado.");
        usuario.Ativo = !usuario.Ativo;
        if (!usuario.Ativo)
        {
            usuario.Disponivel = false;
        }

        _usuarios.Atualizar(usuario);
    }

    public void CadastrarEntregador(string nome, string email, string telefone, string veiculo)
    {
        CadastrarUsuario(nome, email, telefone, Perfil.Entregador, u =>
        {
            u.Veiculo = veiculo;
            u.Disponivel = true;
        });
    }

    public List<Restaurante> ListarRestaurantes() => _restaurantes.ListarTodos();

    public void AtivarOuDesativarRestaurante(int restauranteId)
    {
        var restaurante = _restaurantes.BuscarPorId(restauranteId) ?? throw new RegraException("Restaurante não encontrado.");
        restaurante.Ativo = !restaurante.Ativo;
        if (!restaurante.Ativo)
        {
            restaurante.Aberto = false;
        }

        _restaurantes.Atualizar(restaurante);
    }

    /// <summary>Cria o gerente (login) e o restaurante. Senha inicial: 123456.</summary>
    public void CadastrarRestaurante(string nome, string descricao, string endereco, decimal taxaEntrega,
        string gerenteNome, string gerenteEmail)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new RegraException("Informe o nome do restaurante.");
        }

        if (taxaEntrega < 0)
        {
            throw new RegraException("A taxa de entrega não pode ser negativa.");
        }

        var gerente = CadastrarUsuario(gerenteNome, gerenteEmail, "", Perfil.Restaurante, null);
        _restaurantes.Adicionar(new Restaurante
        {
            Nome = nome.Trim(),
            Descricao = descricao?.Trim() ?? "",
            Endereco = endereco?.Trim() ?? "",
            TaxaEntrega = taxaEntrega,
            GerenteId = gerente.Id
        });
    }

    public List<Pedido> ListarPedidos(StatusPedido? filtro = null)
    {
        var pedidos = _pedidos.ListarTodos();
        return filtro is null ? pedidos : pedidos.Where(p => p.Status == filtro).ToList();
    }

    /// <summary>Números do painel do administrador.</summary>
    public Dictionary<string, string> Indicadores()
    {
        var usuarios = _usuarios.ListarTodos();
        var restaurantes = _restaurantes.ListarTodos();
        var pedidos = _pedidos.ListarTodos();
        var entregues = pedidos.Where(p => p.Status == StatusPedido.Entregue).ToList();

        return new Dictionary<string, string>
        {
            ["Clientes"] = usuarios.Count(u => u.Perfil == Perfil.Cliente).ToString(),
            ["Entregadores"] = usuarios.Count(u => u.Perfil == Perfil.Entregador).ToString(),
            ["Restaurantes ativos"] = restaurantes.Count(r => r.Ativo).ToString(),
            ["Pedidos hoje"] = pedidos.Count(p => p.CriadoEm.Date == DateTime.Today).ToString(),
            ["Em andamento"] = pedidos.Count(p => p.Status != StatusPedido.Entregue && p.Status != StatusPedido.Cancelado).ToString(),
            ["Faturamento"] = entregues.Sum(p => p.Total).ToString("C"),
            ["Comissão da plataforma"] = entregues.Sum(p => p.Comissao).ToString("C")
        };
    }

    private Usuario CadastrarUsuario(string nome, string email, string telefone, Perfil perfil, Action<Usuario>? extra)
    {
        email = (email ?? "").Trim().ToLower();
        if (string.IsNullOrWhiteSpace(nome) || !email.Contains('@'))
        {
            throw new RegraException("Informe nome e e-mail válidos.");
        }

        if (_usuarios.EmailExiste(email))
        {
            throw new RegraException("Já existe uma conta com este e-mail.");
        }

        var usuario = new Usuario
        {
            Nome = nome.Trim(),
            Email = email,
            Telefone = telefone ?? "",
            SenhaHash = Seguranca.HashSenha(DatabaseRepository.SenhaPadrao),
            Perfil = perfil
        };
        extra?.Invoke(usuario);
        _usuarios.Adicionar(usuario);
        return usuario;
    }
}
