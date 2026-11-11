using DeliveryMarketplace.Data;
using DeliveryMarketplace.Models;
using Microsoft.EntityFrameworkCore;

namespace DeliveryMarketplace.Repositories;

public class UsuarioRepository : Repository<Usuario>
{
    public Usuario? BuscarPorEmail(string email)
    {
        using var db = new AppDbContext();
        return db.Usuarios.AsNoTracking().FirstOrDefault(u => u.Email == email);
    }

    public bool EmailExiste(string email)
    {
        using var db = new AppDbContext();
        return db.Usuarios.Any(u => u.Email == email);
    }

    public List<Usuario> ListarPorPerfil(Perfil perfil)
    {
        using var db = new AppDbContext();
        return db.Usuarios.AsNoTracking().Where(u => u.Perfil == perfil).ToList();
    }
}

public class RestauranteRepository : Repository<Restaurante>
{
    public override List<Restaurante> ListarTodos()
    {
        using var db = new AppDbContext();
        return db.Restaurantes.AsNoTracking().Include(r => r.Gerente).OrderBy(r => r.Nome).ToList();
    }

    public List<Restaurante> ListarAtivos()
    {
        using var db = new AppDbContext();
        return db.Restaurantes.AsNoTracking().Where(r => r.Ativo).OrderByDescending(r => r.Aberto).ThenBy(r => r.Nome).ToList();
    }

    public Restaurante? BuscarPorGerente(int gerenteId)
    {
        using var db = new AppDbContext();
        return db.Restaurantes.AsNoTracking().FirstOrDefault(r => r.GerenteId == gerenteId);
    }
}

public class CategoriaRepository : Repository<Categoria>
{
    public List<Categoria> ListarPorRestaurante(int restauranteId)
    {
        using var db = new AppDbContext();
        return db.Categorias.AsNoTracking().Where(c => c.RestauranteId == restauranteId).OrderBy(c => c.Nome).ToList();
    }
}

public class ProdutoRepository : Repository<Produto>
{
    public List<Produto> ListarPorRestaurante(int restauranteId, bool somenteDisponiveis = false)
    {
        using var db = new AppDbContext();
        var query = db.Produtos.AsNoTracking().Include(p => p.Categoria).Where(p => p.RestauranteId == restauranteId);
        if (somenteDisponiveis)
        {
            query = query.Where(p => p.Disponivel);
        }

        return query.OrderBy(p => p.CategoriaId).ThenBy(p => p.Nome).ToList();
    }
}

public class PedidoRepository : Repository<Pedido>
{
    /// <summary>Consulta base: traz cliente, restaurante, entregador, itens e pagamento junto.</summary>
    private static IQueryable<Pedido> Completo(AppDbContext db) => db.Pedidos.AsNoTracking()
        .Include(p => p.Cliente)
        .Include(p => p.Restaurante)
        .Include(p => p.Entregador)
        .Include(p => p.Itens)
        .Include(p => p.Pagamento);

    // A ordenação é feita em memória (o SQLite não ordena DateTime/decimal convertidos pelo EF).
    private static List<Pedido> Ordenar(IEnumerable<Pedido> pedidos) => pedidos.OrderByDescending(p => p.CriadoEm).ToList();

    public override List<Pedido> ListarTodos()
    {
        using var db = new AppDbContext();
        return Ordenar(Completo(db).ToList());
    }

    public Pedido? BuscarCompleto(int id)
    {
        using var db = new AppDbContext();
        return Completo(db).FirstOrDefault(p => p.Id == id);
    }

    public List<Pedido> ListarPorCliente(int clienteId)
    {
        using var db = new AppDbContext();
        return Ordenar(Completo(db).Where(p => p.ClienteId == clienteId).ToList());
    }

    public List<Pedido> ListarPorRestaurante(int restauranteId)
    {
        using var db = new AppDbContext();
        return Ordenar(Completo(db).Where(p => p.RestauranteId == restauranteId).ToList());
    }

    public List<Pedido> ListarPorEntregador(int entregadorId)
    {
        using var db = new AppDbContext();
        return Ordenar(Completo(db).Where(p => p.EntregadorId == entregadorId).ToList());
    }

    /// <summary>Pedidos já aceitos pelo restaurante que ainda não têm entregador.</summary>
    public List<Pedido> ListarSemEntregador()
    {
        using var db = new AppDbContext();
        return Ordenar(Completo(db).Where(p => p.EntregadorId == null &&
            (p.Status == StatusPedido.Confirmado || p.Status == StatusPedido.EmPreparo || p.Status == StatusPedido.ProntoParaRetirada)).ToList());
    }

    public List<Pedido> ListarPorPeriodo(DateTime de, DateTime ate)
    {
        using var db = new AppDbContext();
        var inicio = de.Date;
        var fim = ate.Date.AddDays(1);
        return Ordenar(Completo(db).Where(p => p.CriadoEm >= inicio && p.CriadoEm < fim).ToList());
    }

    /// <summary>Grava um pedido novo com seus itens e o pagamento (tudo de uma vez).</summary>
    public void Criar(Pedido pedido)
    {
        using var db = new AppDbContext();
        db.Pedidos.Add(pedido);
        db.SaveChanges();
    }

    /// <summary>Atualiza só os campos de status/entregador do pedido e do pagamento.</summary>
    public void AtualizarStatus(Pedido pedido)
    {
        using var db = new AppDbContext();
        var atual = db.Pedidos.Include(p => p.Pagamento).First(p => p.Id == pedido.Id);
        atual.Status = pedido.Status;
        atual.EntregadorId = pedido.EntregadorId;
        atual.EntregueEm = pedido.EntregueEm;
        if (atual.Pagamento is not null && pedido.Pagamento is not null)
        {
            atual.Pagamento.Status = pedido.Pagamento.Status;
            atual.Pagamento.CodigoTransacao = pedido.Pagamento.CodigoTransacao;
        }

        db.SaveChanges();
    }
}
