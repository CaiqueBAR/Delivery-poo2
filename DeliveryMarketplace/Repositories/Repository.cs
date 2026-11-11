using DeliveryMarketplace.Data;
using Microsoft.EntityFrameworkCore;

namespace DeliveryMarketplace.Repositories;

/// <summary>
/// Repositório genérico com as operações básicas (CRUD).
/// Cada método abre um contexto curto, faz a operação e fecha.
/// Os repositórios específicos herdam dele e acrescentam as consultas próprias.
/// </summary>
public class Repository<T> where T : class
{
    public virtual List<T> ListarTodos()
    {
        using var db = new AppDbContext();
        return db.Set<T>().AsNoTracking().ToList();
    }

    public virtual T? BuscarPorId(int id)
    {
        using var db = new AppDbContext();
        return db.Set<T>().Find(id);
    }

    public void Adicionar(T entidade)
    {
        using var db = new AppDbContext();
        db.Set<T>().Add(entidade);
        db.SaveChanges();
    }

    public void Atualizar(T entidade)
    {
        using var db = new AppDbContext();
        db.Set<T>().Update(entidade);
        db.SaveChanges();
    }

    public void Remover(T entidade)
    {
        using var db = new AppDbContext();
        db.Set<T>().Remove(entidade);
        db.SaveChanges();
    }
}
