using Microsoft.EntityFrameworkCore;

namespace CSharpLocalAndRemote.Mapper;

// El EntityManager es la clase que se encarga de la conexión con la base de datos.
// https://learn.microsoft.com/es-es/ef/core/
public class EntityManager<T> where T : class
{
    public EntityManager(DbContext context)
    {
        Context = context;
        DbSet = Context.Set<T>();
    }

    public DbSet<T> DbSet { get; }
    public DbContext Context { get; }
}