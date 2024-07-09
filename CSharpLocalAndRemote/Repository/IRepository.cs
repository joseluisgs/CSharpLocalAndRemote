using CSharpFunctionalExtensions;

namespace CSharpLocalAndRemote.Repository;

public interface IRepository<TId, T, TE>
{
    Task<Result<List<T>, TE>> GetAllAsync();

    Task<Result<T, TE>> GetByIdAsync(TId id);

    Task<Result<T, TE>> SaveAsync(T entity);

    Task<Result<T, TE>> UpdateAsync(TId id, T entity);

    Task<Result<TId, TE>> DeleteAsync(TId id);
}