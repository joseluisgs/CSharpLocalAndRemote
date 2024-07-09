using CSharpFunctionalExtensions;

namespace CSharpLocalAndRemote.Storage;

public interface ISerializationStorage<T, TE>
{
    Task<Result<List<T>, TE>> ImportAsync(FileInfo file);
    Task<Result<int, TE>> ExportAsync(FileInfo file, List<T> data);
}