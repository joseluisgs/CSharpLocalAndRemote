using CSharpFunctionalExtensions;
using CSharpLocalAndRemote.Error;
using CSharpLocalAndRemote.model;

namespace CSharpLocalAndRemote.Service;

public interface ITenistasService
{
    const long RefreshTime = 50000;

    Task<Result<List<Tenista>, TenistaError>> GetAllAsync(bool fromRemote);

    Task<Result<Tenista, TenistaError>> GetByIdAsync(long id);

    Task<Result<Tenista, TenistaError>> SaveAsync(Tenista tenista);

    Task<Result<Tenista, TenistaError>> UpdateAsync(long id, Tenista tenista);

    Task<Result<long, TenistaError>> DeleteAsync(long id);

    Task<Result<int, TenistaError>> ImportDataAsync(FileInfo file);

    Task<Result<int, TenistaError>> ExportDataAsync(FileInfo file, bool fromRemote);

    void EnableAutoRefresh();

    void DisableAutoRefresh();

    void LoadData();
}