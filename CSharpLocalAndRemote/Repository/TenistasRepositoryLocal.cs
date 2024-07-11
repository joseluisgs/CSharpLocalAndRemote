using CSharpFunctionalExtensions;
using CSharpLocalAndRemote.Database;
using CSharpLocalAndRemote.Error;
using CSharpLocalAndRemote.Logger;
using CSharpLocalAndRemote.Mapper;
using CSharpLocalAndRemote.model;
using Microsoft.EntityFrameworkCore;

namespace CSharpLocalAndRemote.Repository;

public class TenistasRepositoryLocal : ITenistasRepositoryLocal
{
    private readonly DbContext _db;
    private readonly Serilog.Core.Logger _logger = LoggerUtils<TenistasRepositoryLocal>.GetLogger();

    public TenistasRepositoryLocal(DbContext dbContext)
    {
        _db = dbContext;
        Init();
    }

    public async Task<Result<List<Tenista>, TenistaError>> GetAllAsync()
    {
        _logger.Debug("Obteniendo todos los tenistas locales en bd");
        try
        {
            var tenistas = await _db.Set<TenistaEntity>() // Obtenemos todos los tenistas de la base de datos
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
        _logger.Debug("Obteniendo el tenista local en bd con id {id}", id);
        try
        {
            var entityToFind = await _db.Set<TenistaEntity>().FindAsync(id); // Buscamos el tenista por id
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
        _logger.Debug("Guardando el tenista local en bd");
        try
        {
            var timeStamp = DateTime.Now.ToString("o"); // Obtenemos la fecha y hora actual
            var entityToSave = entity.ToTenistaEntity();

            entityToSave.CreatedAt = timeStamp; // Añadimos la fecha de creación
            entityToSave.UpdatedAt = timeStamp; // Añadimos la fecha de actualización
            entityToSave.Id = Tenista.NewId;
            await _db.Set<TenistaEntity>().AddAsync(entityToSave); // Añadimos el tenista a la base de datos
            await _db.SaveChangesAsync(); // Guardamos los cambios

            _logger.Debug("Guardado el tenista local en bd {entityToSave}", entityToSave);
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
        _logger.Debug("Actualizando el tenista local en bd con id {id}", id);
        try
        {
            var entityToUpdate = await _db.Set<TenistaEntity>().FindAsync(id);
            if (entityToUpdate == null)
                return Result.Failure<Tenista, TenistaError>(new TenistaError.DatabaseError(
                    $"El tenista con id {id} no se encontró en la base de datos."));

            var timeStamp = DateTime.Now.ToString("o"); // Obtenemos la fecha y hora actual
            entityToUpdate.UpdateFrom(entity.ToTenistaEntity()); // Actualizamos los campos de la entidad
            entityToUpdate.UpdatedAt = timeStamp; // Actualizamos la fecha de actualización

            await _db.SaveChangesAsync(); // Guardamos los cambios, uso Save porque la entidad ya existe en el contexto (FindAsync)
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
        _logger.Debug("Borrando el tenista local en bd con id {id}", id);
        try
        {
            var entityToDelete = await _db.Set<TenistaEntity>().FindAsync(id);
            if (entityToDelete == null)
                return Result.Failure<long, TenistaError>(new TenistaError.DatabaseError(
                    $"El tenista con id {id} no se encontró en la base de datos."));

            _db.Set<TenistaEntity>()
                .Remove(entityToDelete); // Borramos el tenista de la base de datos, uso Remove porque la entidad ya existe en el contexto (FindAsync)
            await _db.SaveChangesAsync();
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
        _logger.Debug("Inicializando el repositorio local");
        await _db.Database.EnsureCreatedAsync(); // Creamos la base de datos si no existe
        await _db.SaveChangesAsync(); // Guardamos los cambios
        await _db.RemoveAllAsync(); // Borramos todos los registros porque es un repositorio local
        await _db.SaveChangesAsync(); // Guardamos los cambios
    }

    public async Task<Result<int, TenistaError>> SaveAllAsync(List<Tenista> tenistas)
    {
        _logger.Debug("Guardando todos los tenistas locales en bd");

        var entityList =
            tenistas.Select(tenista => tenista.ToTenistaEntity()).ToList(); // Convertimos los tenistas a entidades
        var timeStamp = DateTime.Now.ToString("o"); // Obtenemos la fecha y hora actual
        foreach (var entity in entityList)
        {
            entity.CreatedAt = timeStamp; // Añadimos la fecha de creación
            entity.UpdatedAt = timeStamp; // Añadimos la fecha de actualización
            entity.Id = Tenista.NewId;
        }

        await _db.Set<TenistaEntity>().AddRangeAsync(entityList); // Añadimos los tenistas a la base de datos
        await _db.SaveChangesAsync(); // Guardamos los cambios
        return Result.Success<int, TenistaError>(entityList.Count);
    }

    public async Task<Result<bool, TenistaError>> RemoveAllAsync()
    {
        _logger.Debug("Borrando todos los tenistas locales en bd");

        await _db.RemoveAllAsync(); // Borramos todos los registros porque es un repositorio local
        await _db.SaveChangesAsync(); // Guardamos los cambios
        return Result.Success<bool, TenistaError>(true);
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