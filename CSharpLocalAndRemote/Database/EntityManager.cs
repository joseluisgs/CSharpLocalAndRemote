using Microsoft.EntityFrameworkCore;

namespace CSharpLocalAndRemote.Database;

// El EntityManager es la clase que se encarga de la conexión con la base de datos.
// https://learn.microsoft.com/es-es/ef/core/
public class EntityManager<T> where T : class
{
    public EntityManager(DbContext context)
    {
        Context = context;
    }

    public DbSet<T> DbSet =>
        Context.Set<T>(); // DbSet es una colección de entidades que se pueden consultar, agregar, modificar y eliminar.

    public DbContext Context { get; } // DbContext es una clase que se encarga de la conexión con la base de datos.
}