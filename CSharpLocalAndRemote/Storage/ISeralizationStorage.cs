using CSharpFunctionalExtensions;

namespace CSharpLocalAndRemote.Storage;

public interface ISerializationStorage<T, E>
{
    Task<Result<List<T>, E>> ImportAsync(FileInfo file);
    Task<Result<int, E>> ExportAsync(FileInfo file, List<T> data);
}