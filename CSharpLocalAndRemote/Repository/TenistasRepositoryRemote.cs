using CSharpFunctionalExtensions;
using CSharpLocalAndRemote.Error;
using CSharpLocalAndRemote.Logger;
using CSharpLocalAndRemote.model;

namespace CSharpLocalAndRemote.Repository;

public class TenistasRepositoryRemote : ITenistasRepository
{
    private readonly Serilog.Core.Logger _logger = LoggerUtils<TenistasRepositoryLocal>.GetLogger();

    public Task<Result<List<Tenista>, TenistaError>> GetAllAsync()
    {
        _logger.Debug("Obteniendo todos los tenistas remotos");
        throw new NotImplementedException();
    }

    public Task<Result<Tenista, TenistaError>> GetByIdAsync(long id)
    {
        _logger.Debug("Obteniendo el tenista remoto con id {id}", id);
        throw new NotImplementedException();
    }

    public Task<Result<Tenista, TenistaError>> SaveAsync(Tenista entity)
    {
        _logger.Debug("Guardando el tenista remoto {entity}", entity);
        throw new NotImplementedException();
    }

    public Task<Result<Tenista, TenistaError>> UpdateAsync(long id, Tenista entity)
    {
        _logger.Debug("Actualizando el tenista remoto con id {id}", id);
        throw new NotImplementedException();
    }

    public Task<Result<long, TenistaError>> DeleteAsync(long id)
    {
        _logger.Debug("Eliminando el tenista remoto con id {id}", id);
        throw new NotImplementedException();
    }
}