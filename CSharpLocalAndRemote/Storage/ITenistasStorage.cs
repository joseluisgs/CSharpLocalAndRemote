using CSharpLocalAndRemote.Error;
using CSharpLocalAndRemote.model;

namespace CSharpLocalAndRemote.Storage;

public interface ITenistasStorage : ISerializationStorage<Tenista, TenistaError.StorageError>
{
}