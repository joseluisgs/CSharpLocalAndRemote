using CSharpLocalAndRemote.Error;
using CSharpLocalAndRemote.model;

namespace CSharpLocalAndRemote.Repository;

public interface ITenistasRepository : IRepository<long, Tenista, TenistaError>
{
}