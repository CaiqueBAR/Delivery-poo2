using DeliveryMarketplace.Data;
using DeliveryMarketplace.Models;

namespace DeliveryMarketplace.Repositories;

/// <summary>
/// Repositório responsável por CRIAR o banco de dados.
/// Na primeira execução cria o arquivo delivery.db com todas as tabelas
/// e insere os dados fictícios de demonstração.
/// </summary>
public class DatabaseRepository
{
    public const string SenhaPadrao = "123456";

    public bool BancoExiste() => File.Exists(AppDbContext.CaminhoBanco);

    public string Caminho => AppDbContext.CaminhoBanco;

    /// <summary>Cria as tabelas (se ainda não existirem) e popula com dados de exemplo.</summary>
    public void CriarBanco()
    {
        using var db = new AppDbContext();
        db.Database.EnsureCreated();

        if (!db.Usuarios.Any())
        {
            InserirDadosFicticios(db);
        }
    }

    /// <summary>Apaga e recria o banco do zero (útil para voltar aos dados de demonstração).</summary>
    public void RecriarBanco()
    {
        using (var db = new AppDbContext())
        {
            db.Database.EnsureDeleted();
        }

        CriarBanco();
    }

    public Dictionary<string, int> ContarRegistros()
    {
        using var db = new AppDbContext();
        return new Dictionary<string, int>
        {
            ["Usuários"] = db.Usuarios.Count(),
            ["Restaurantes"] = db.Restaurantes.Count(),
            ["Categorias"] = db.Categorias.Count(),
            ["Produtos"] = db.Produtos.Count(),
            ["Pedidos"] = db.Pedidos.Count(),
            ["Itens de pedido"] = db.ItensPedido.Count(),
            ["Pagamentos"] = db.Pagamentos.Count()
        };
    }

    private static void InserirDadosFicticios(AppDbContext db)
    {
        var senha = Seguranca.HashSenha(SenhaPadrao);

        // ===== Usuários =====
        var admin = Usuario("Ana Administradora", "admin@delivery.local", Perfil.Administrador);
        var gerentes = new[]
        {
            Usuario("Carlos Gerente", "pizzaria@delivery.local", Perfil.Restaurante),
            Usuario("Beatriz Gerente", "burger@delivery.local", Perfil.Restaurante),
            Usuario("Daniel Gerente", "sushi@delivery.local", Perfil.Restaurante)
        };
        var clientes = new[]
        {
            Cliente("Fernanda Cliente", "fernanda@delivery.local", "Rua Augusta, 1500 - Consolação, São Paulo/SP"),
            Cliente("Gustavo Cliente", "gustavo@delivery.local", "Rua Vergueiro, 300 - Liberdade, São Paulo/SP"),
            Cliente("Helena Cliente", "helena@delivery.local", "Alameda Santos, 45 - Jardim Paulista, São Paulo/SP")
        };
        var entregadores = new[]
        {
            Entregador("João Entregador", "joao@delivery.local", "Moto · ABC1D23"),
            Entregador("Karina Entregadora", "karina@delivery.local", "Bicicleta")
        };

        db.Usuarios.Add(admin);
        db.Usuarios.AddRange(gerentes);
        db.Usuarios.AddRange(clientes);
        db.Usuarios.AddRange(entregadores);
        db.SaveChanges();

        // ===== Restaurantes e cardápios =====
        var pizzaria = Restaurante(db, gerentes[0], "Pizzaria Forno de Pedra", "Pizzas artesanais em forno a lenha.", 7.90m,
            ("Pizzas", [("Margherita", 49.90m), ("Calabresa", 47.90m), ("Quatro queijos", 56.90m)]),
            ("Bebidas", [("Refrigerante lata", 6.50m), ("Suco natural", 9.90m)]));

        var burger = Restaurante(db, gerentes[1], "Burger da Esquina", "Hambúrgueres smash e batatas crocantes.", 5.00m,
            ("Hambúrgueres", [("Smash clássico", 29.90m), ("Duplo bacon", 38.90m), ("Veggie", 31.90m)]),
            ("Acompanhamentos", [("Batata frita", 14.90m), ("Onion rings", 16.90m)]),
            ("Bebidas", [("Milkshake", 18.90m)]));

        var sushi = Restaurante(db, gerentes[2], "Sushi Kaze", "Culinária japonesa contemporânea.", 9.90m,
            ("Combinados", [("Combo 20 peças", 79.90m), ("Combo 40 peças", 139.90m)]),
            ("Temakis", [("Temaki salmão", 32.90m), ("Temaki skin", 27.90m)]),
            ("Bebidas", [("Chá verde gelado", 8.90m)]));

        // ===== Pedidos de exemplo em vários status (datas dos últimos dias) =====
        Pedido(db, clientes[0], pizzaria, [0, 3], MetodoPagamento.Pix, StatusPedido.Entregue, entregadores[0], 5);
        Pedido(db, clientes[1], burger, [0, 3, 5], MetodoPagamento.CartaoCredito, StatusPedido.Entregue, entregadores[1], 4);
        Pedido(db, clientes[2], sushi, [0], MetodoPagamento.Pix, StatusPedido.Entregue, entregadores[0], 3);
        Pedido(db, clientes[0], burger, [1, 3], MetodoPagamento.Dinheiro, StatusPedido.Entregue, entregadores[1], 2);
        Pedido(db, clientes[1], sushi, [2, 4], MetodoPagamento.Pix, StatusPedido.Cancelado, null, 1);
        Pedido(db, clientes[2], pizzaria, [2, 4], MetodoPagamento.CartaoDebito, StatusPedido.EmRota, entregadores[0], 0);
        Pedido(db, clientes[0], sushi, [2], MetodoPagamento.Dinheiro, StatusPedido.EmPreparo, null, 0);
        Pedido(db, clientes[1], pizzaria, [1], MetodoPagamento.Pix, StatusPedido.AguardandoConfirmacao, null, 0);
        db.SaveChanges();

        Usuario Usuario(string nome, string email, Perfil perfil) => new()
        {
            Nome = nome, Email = email, SenhaHash = senha, Telefone = "(11) 90000-0000", Perfil = perfil
        };

        Usuario Cliente(string nome, string email, string endereco)
        {
            var u = Usuario(nome, email, Perfil.Cliente);
            u.Endereco = endereco;
            u.Cpf = "000.000.000-00";
            return u;
        }

        Usuario Entregador(string nome, string email, string veiculo)
        {
            var u = Usuario(nome, email, Perfil.Entregador);
            u.Veiculo = veiculo;
            u.Disponivel = true;
            return u;
        }
    }

    private static Restaurante Restaurante(AppDbContext db, Usuario gerente, string nome, string descricao, decimal taxa,
        params (string categoria, (string nome, decimal preco)[] produtos)[] cardapio)
    {
        var restaurante = new Restaurante
        {
            Nome = nome,
            Descricao = descricao,
            Cnpj = "00.000.000/0001-00",
            Telefone = "(11) 3000-0000",
            Endereco = "São Paulo/SP",
            TaxaEntrega = taxa,
            Aberto = true,
            GerenteId = gerente.Id
        };

        foreach (var (nomeCategoria, produtos) in cardapio)
        {
            var categoria = new Categoria { Nome = nomeCategoria, Restaurante = restaurante };
            restaurante.Categorias.Add(categoria);
            foreach (var (nomeProduto, preco) in produtos)
            {
                restaurante.Produtos.Add(new Produto
                {
                    Nome = nomeProduto, Descricao = $"{nomeProduto} da casa", Preco = preco, Categoria = categoria
                });
            }
        }

        db.Restaurantes.Add(restaurante);
        db.SaveChanges();
        return restaurante;
    }

    private static void Pedido(AppDbContext db, Usuario cliente, Restaurante restaurante, int[] produtos,
        MetodoPagamento metodo, StatusPedido status, Usuario? entregador, int diasAtras)
    {
        var data = DateTime.Now.AddDays(-diasAtras).AddHours(-1);
        var pedido = new Pedido
        {
            ClienteId = cliente.Id,
            RestauranteId = restaurante.Id,
            EntregadorId = entregador?.Id,
            EnderecoEntrega = cliente.Endereco ?? "",
            TaxaEntrega = restaurante.TaxaEntrega,
            Status = status,
            CriadoEm = data,
            EntregueEm = status == StatusPedido.Entregue ? data.AddMinutes(40) : null
        };

        foreach (var indice in produtos)
        {
            var produto = restaurante.Produtos[indice];
            pedido.Itens.Add(new ItemPedido
            {
                ProdutoId = produto.Id, NomeProduto = produto.Nome, PrecoUnitario = produto.Preco, Quantidade = 1
            });
        }

        pedido.Subtotal = pedido.Itens.Sum(i => i.Subtotal);
        pedido.Total = pedido.Subtotal + pedido.TaxaEntrega;
        pedido.Pagamento = new Pagamento
        {
            Metodo = metodo,
            Valor = pedido.Total,
            CriadoEm = data,
            Status = status switch
            {
                StatusPedido.Cancelado => StatusPagamento.Estornado,
                _ when metodo == MetodoPagamento.Dinheiro && status != StatusPedido.Entregue => StatusPagamento.Pendente,
                _ => StatusPagamento.Aprovado
            },
            CodigoTransacao = $"DEMO-{Guid.NewGuid().ToString("N")[..8].ToUpper()}"
        };

        db.Pedidos.Add(pedido);
    }
}
