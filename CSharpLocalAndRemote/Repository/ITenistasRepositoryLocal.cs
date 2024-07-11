using CSharpFunctionalExtensions;
using CSharpLocalAndRemote.Error;
using CSharpLocalAndRemote.model;

namespace CSharpLocalAndRemote.Repository;

public interface ITenistasRepositoryLocal : ITenistasRepository
{
    Task<Result<bool, TenistaError>> RemoveAllAsync();
    Task<Result<int, TenistaError>> SaveAllAsync(List<Tenista> remoteTenistas);
}