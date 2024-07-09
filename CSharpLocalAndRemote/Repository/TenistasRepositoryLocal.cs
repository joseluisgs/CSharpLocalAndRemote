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
            var entityToUpdate = await Db.Set<TenistaEntity>().FindAsync(id);
            if (entityToUpdate == null)
                return Result.Failure<Tenista, TenistaError>(new TenistaError.DatabaseError(
                    $"El tenista con id {id} no se encontró en la base de datos."));

            var timeStamp = DateTime.Now.ToString("o"); // Obtenemos la fecha y hora actual
            entityToUpdate.UpdateFrom(entity.ToTenistaEntity()); // Actualizamos los campos de la entidad
            entityToUpdate.UpdatedAt = timeStamp; // Actualizamos la fecha de actualización

            await Db.SaveChangesAsync(); // Guardamos los cambios
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
        try
        {
            var entityToDelete = await Db.Set<TenistaEntity>().FindAsync(id);
            if (entityToDelete == null)
                return Result.Failure<long, TenistaError>(new TenistaError.DatabaseError(
                    $"El tenista con id {id} no se encontró en la base de datos."));

            Db.Set<TenistaEntity>().Remove(entityToDelete);
            await Db.SaveChangesAsync();
            return Result.Success<long, TenistaError>(id);
        }
        catch (Exception ex)
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

public static class TenistaEntityExtensions
{
    public static void UpdateFrom(this TenistaEntity entity, TenistaEntity model)
    {
        entity.Nombre = model.Nombre;
        entity.Pais = model.Pais;
        entity.Altura = model.Altura;
        entity.Peso = model.Peso;
        entity.Puntos = model.Puntos;
        entity.Mano = model.Mano;
        entity.FechaNacimiento = model.FechaNacimiento;
    }
}