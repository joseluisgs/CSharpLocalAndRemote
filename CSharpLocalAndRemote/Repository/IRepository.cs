using CSharpFunctionalExtensions;

namespace CSharpLocalAndRemote.Repository;

public interface IRepository<ID, T, E>
{
    Task<Result<List<T>, E>> GetAllAsync();

    Task<Result<T, E>> GetByIdAsync(ID id);

    Task<Result<T, E>> SaveAsync(T entity);

    Task<Result<T, E>> UpdateAsync(ID id, T entity);

    Task<Result<ID, E>> DeleteAsync(ID id);
}