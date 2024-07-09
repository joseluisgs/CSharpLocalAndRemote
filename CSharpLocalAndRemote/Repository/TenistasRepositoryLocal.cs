using CSharpFunctionalExtensions;
using CSharpLocalAndRemote.Database;
using CSharpLocalAndRemote.Error;
using CSharpLocalAndRemote.Mapper;
using CSharpLocalAndRemote.model;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Core;

namespace CSharpLocalAndRemote.Repository;

public class TenistasRepositoryLocal : ITenistasRepository
{
    public TenistasRepositoryLocal(DbContext dbContext)
    {
        Db = dbContext;
        Init();
    }

    private DbContext Db { get; }
    private Logger Logger { get; } = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Console().CreateLogger();

    public async Task<Result<List<Tenista>, TenistaError>> GetAllAsync()
    {
        Logger.Debug("Obteniendo todos los tenistas locales en bd");
        try
        {
            var tenistas = await Db.Set<TenistaEntity>() // Obtenemos todos los tenistas de la base de datos
                .Select(entity => entity.ToTenista()) // Convertimos los tenistas de entidad a modelo
                .ToListAsync();

            return Result.Success<List<Tenista>, TenistaError>(tenistas);
        }
        catch (Exception ex)
        {
            return Result.Failure<List<Tenista>, TenistaError>(new TenistaError.DatabaseError(
                $"No se pueden obtener los tenistas locales: {ex.Message}"));
        }
    }

    public async Task<Result<Tenista, TenistaError>> GetByIdAsync(long id)
    {
        Logger.Debug("Obteniendo el tenista local en bd con id {id}", id);
        try
        {
            var entityToFind = await Db.Set<TenistaEntity>().FindAsync(id);
            // Si no se encuentra el tenista, retornamos un error con operador ternario
            return entityToFind is null
                ? Result.Failure<Tenista, TenistaError>(new TenistaError.DatabaseError(
                    $"El tenista con id {id} no se encontró en la base de datos."))
                : Result.Success<Tenista, TenistaError>(entityToFind.ToTenista());
        }
        catch (Exception ex)
        {
            return Result.Failure<Tenista, TenistaError>(new TenistaError.DatabaseError(
                $"No se ha obteniendo el tenista con id {id}: {ex.Message}"));
        }
    }

    public async Task<Result<Tenista, TenistaError>> SaveAsync(Tenista entity)
    {
        Logger.Debug("Guardando el tenista local en bd");
        try
        {
            var timeStamp = DateTime.Now.ToString("o"); // Obtenemos la fecha y hora actual
            var entityToSave = entity.ToTenistaEntity();
            entityToSave.CreatedAt = timeStamp; // Añadimos la fecha de creación
            entityToSave.UpdatedAt = timeStamp; // Añadimos la fecha de actualización
            Db.Set<TenistaEntity>().Add(entityToSave); // Añadimos el tenista a la base de datos
            await Db.SaveChangesAsync(); // Guardamos los cambios
            return Result.Success<Tenista, TenistaError>(entityToSave.ToTenista());
        }
        catch (Exception ex)
        {
            return Result.Failure<Tenista, TenistaError>(new TenistaError.DatabaseError(
                $"No se ha guardando el tenista: {ex.Message}"));
        }
    }

    public async Task<Result<Tenista, TenistaError>> UpdateAsync(long id, Tenista entity)
    {
        Logger.Debug("Actualizando el tenista local en bd con id {id}", id);
        try
        {
            // La buscamos en la base de datos
            var existingEntity = await Db.Set<TenistaEntity>().FindAsync(id);
            if (existingEntity == null)
                return Result.Failure<Tenista, TenistaError>(new TenistaError.DatabaseError(
                    $"El tenista con id {id} no se encontró en la base de datos."));

            var timeStamp = DateTime.Now.ToString("o"); // Obtenemos la fecha y hora actual
            var entityToUpdate = entity.ToTenistaEntity(); // Convertimos el tenista a entidad
            // Actualizamos los campos de la entidad
            existingEntity.Nombre = entityToUpdate.Nombre;
            existingEntity.Pais = entityToUpdate.Pais;
            existingEntity.Altura = entityToUpdate.Altura;
            existingEntity.Peso = entityToUpdate.Peso;
            existingEntity.Puntos = entityToUpdate.Puntos;
            existingEntity.Mano = entityToUpdate.Mano;
            existingEntity.FechaNacimiento = entityToUpdate.FechaNacimiento;
            existingEntity.UpdatedAt = timeStamp; // Actualizamos la fecha de actualización

            // Db.Set<TenistaEntity>().Update(entityToUpdate); // Actualizamos el tenista en la base de datos
            // No usar el método Update por temas de conflictos de simultaneidad https://learn.microsoft.com/es-es/ef/core/saving/concurrency?tabs=data-annotations, prueba a ver quete pasa
            await Db.SaveChangesAsync(); // Guardamos los cambios 
            /*if (res == 0)
                return Result.Failure<Tenista, TenistaError>(new TenistaError.DatabaseError(
                    $"No se ha actualizado el tenista con id {id}"));*/
            return Result.Success<Tenista, TenistaError>(entityToUpdate.ToTenista());
        }
        catch (Exception ex)
        {
            return Result.Failure<Tenista, TenistaError>(new TenistaError.DatabaseError(
                $"No se ha actualizando el tenista con id {id}: {ex.Message}"));
        }
    }

    public async Task<Result<long, TenistaError>> DeleteAsync(long id)
    {
        Logger.Debug("Borrando el tenista local en bd con id {id}", id);
        try // Intentamos borrar el tenista
        {
            var entityToDelete = new TenistaEntity { Id = id }; // Creamos una entidad con el id
            Db.Set<TenistaEntity>().Remove(entityToDelete); // Borramos el tenista de la base de datos
            var res = await Db.SaveChangesAsync(); // Guardamos los cambios
            if (res == 0) // Si no se ha borrado ningún tenista, retornamos un error
                return Result.Failure<long, TenistaError>(new TenistaError.DatabaseError(
                    $"No se ha borrado el tenista con id {id}"));
            return Result.Success<long, TenistaError>(id); // Si se ha borrado, retornamos el id
        }
        catch (Exception ex) // Si hay un error, retornamos un error
        {
            return Result.Failure<long, TenistaError>(new TenistaError.DatabaseError(
                $"No se ha borrando el tenista con id {id}: {ex.Message}"));
        }
    }

    private async void Init()
    {
        Logger.Debug("Inicializando el repositorio local");
        await Db.Database.EnsureCreatedAsync(); // Creamos la base de datos si no existe
        await Db.SaveChangesAsync(); // Guardamos los cambios
        await Db.RemoveAllAsync(); // Borramos todos los registros porque es un repositorio local
    }
}