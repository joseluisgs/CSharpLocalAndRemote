using CSharpFunctionalExtensions;
using CSharpLocalAndRemote.Error;
using CSharpLocalAndRemote.Logger;
using CSharpLocalAndRemote.Mapper;
using CSharpLocalAndRemote.model;
using CSharpLocalAndRemote.Rest;

namespace CSharpLocalAndRemote.Repository;

public class TenistasRepositoryRemote : ITenistasRepository
{
    private readonly ITenistasApiRest _api;
    private readonly Serilog.Core.Logger _logger = LoggerUtils<TenistasRepositoryLocal>.GetLogger();

    public TenistasRepositoryRemote(ITenistasApiRest api)
    {
        _api = api;
    }

    public async Task<Result<List<Tenista>, TenistaError>> GetAllAsync()
    {
        _logger.Debug("Obteniendo todos los tenistas remotos");
        try
        {
            var tenistas = (await _api.GetAllAsync()) // Obtenemos todos los tenistas remotos
                .Select(dto => dto.ToTenista()) // Convertimos los tenistas de dto a modelo
                .ToList(); // Convertimos a lista

            return Result.Success<List<Tenista>, TenistaError>(tenistas);
        }
        catch (Exception ex)
        {
            return Result.Failure<List<Tenista>, TenistaError>(new TenistaError.RemoteError(
                $"No se pueden obtener los tenistas remotos: {ex.Message}"));
        }
    }

    public async Task<Result<Tenista, TenistaError>> GetByIdAsync(long id)
    {
        _logger.Debug("Obteniendo el tenista remoto con id {id}", id);

        try
        {
            var tenista = (await _api.GetByIdAsync(id)).ToTenista();

            return Result.Success<Tenista, TenistaError>(tenista);
        }
        catch (Exception ex)
        {
            return Result.Failure<Tenista, TenistaError>(new TenistaError.RemoteError(
                $"No se ha obteniendo el tenista remoto con id {id}: {ex.Message}"));
        }
    }

    public async Task<Result<Tenista, TenistaError>> SaveAsync(Tenista entity)
    {
        _logger.Debug("Guardando el tenista remoto {entity}", entity);
        try
        {
            var timeStamp = DateTime.Now.ToString("o"); // Obtenemos la fecha y hora actual
            var entityToSave = entity.ToTenistaDto() with
            {
                Id = Tenista.NewId, // Añadimos un id nuevo
                CreatedAt = timeStamp, // Añadimos la fecha de creación
                UpdatedAt = timeStamp // Añadimos la fecha de actualización
            };

            var tenista = (await _api.SaveAsync(entityToSave)).ToTenista();
            return Result.Success<Tenista, TenistaError>(tenista);
        }
        catch (Exception ex)
        {
            return Result.Failure<Tenista, TenistaError>(new TenistaError.RemoteError(
                $"No se ha guardado el tenista remoto {entity}: {ex.Message}"));
        }
    }

    public async Task<Result<Tenista, TenistaError>> UpdateAsync(long id, Tenista entity)
    {
        _logger.Debug("Actualizando el tenista remoto con id {id}", id);

        try
        {
            var timeStamp = DateTime.Now.ToString("o"); // Obtenemos la fecha y hora actual
            var entityToUpdate = entity.ToTenistaDto() with
            {
                Id = id, // Añadimos el id
                UpdatedAt = timeStamp // Añadimos la fecha de actualización
            };

            var tenista = (await _api.UpdateAsync(id, entityToUpdate)).ToTenista();
            return Result.Success<Tenista, TenistaError>(tenista);
        }
        catch (Exception ex)
        {
            return Result.Failure<Tenista, TenistaError>(new TenistaError.RemoteError(
                $"No se ha actualizado el tenista remoto con id {id}: {ex.Message}"));
        }
    }

    public async Task<Result<long, TenistaError>> DeleteAsync(long id)
    {
        _logger.Debug("Eliminando el tenista remoto con id {id}", id);
        try
        {
            await _api.DeleteAsync(id);

            return Result.Success<long, TenistaError>(id);
        }
        catch (Exception ex)
        {
            return Result.Failure<long, TenistaError>(new TenistaError.RemoteError(
                $"No se ha eliminado el tenista remoto con id {id}: {ex.Message}"));
        }
    }
}