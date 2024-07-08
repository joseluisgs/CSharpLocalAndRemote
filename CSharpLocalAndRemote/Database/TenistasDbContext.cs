using CSharpLocalAndRemote.Database;
using Microsoft.EntityFrameworkCore;

namespace CSharpLocalAndRemote.Mapper;

// El DbContext es la clase que se encarga de la conexión con la base de datos.
public class TenistasDbContext : DbContext
{
    public TenistasDbContext(DbContextOptions<DbContext> options) : base(options)
    {
    }

    public DbSet<TenistaEntity> Tenistas { get; set; } // DbSet es una colección de entidades de tipo TenistaEntity

    // En el método OnModelCreating se definen las relaciones entre las entidades y las tablas de la base de datos.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TenistaEntity>(entity =>
        {
            // Se define la tabla y la clave primaria TenistaEntity
            entity.ToTable("TenistaEntity");
            entity.HasKey(e => e.Id); // Se define la clave primaria de la tabla
            entity.Property(e => e.Nombre).IsRequired(); // Se define que el campo Nombre es obligatorio
            entity.Property(e => e.Pais).IsRequired(); // Se define que el campo Pais es obligatorio
            entity.Property(e => e.Altura).IsRequired(); // Se define que el campo Altura es obligatorio
            entity.Property(e => e.Peso).IsRequired(); // Se define que el campo Peso es obligatorio
            entity.Property(e => e.Puntos).IsRequired(); // Se define que el campo Puntos es obligatorio
            entity.Property(e => e.Mano).IsRequired(); // Se define que el campo Mano es obligatorio
            entity.Property(e => e.FechaNacimiento)
                .IsRequired(); // Se define que el campo FechaNacimiento es obligatorio
            entity.Property(e => e.CreatedAt).IsRequired()
                .ValueGeneratedOnAdd(); // Se define que el campo CreatedAt es obligatorio y se genera automáticamente al añadir un registro
            entity.Property(e => e.UpdatedAt).IsRequired()
                .ValueGeneratedOnUpdate(); // Se define que el campo UpdatedAt es obligatorio y se genera automáticamente al actualizar un registro
            entity.Property(e => e.IsDeleted).IsRequired()
                .HasDefaultValue(
                    false); // Se define que el campo IsDeleted es obligatorio y tiene un valor por defecto de false
        });
    }
}