using CSharpFunctionalExtensions;
using CSharpLocalAndRemote.Error;
using CSharpLocalAndRemote.model;

namespace CSharpLocalAndRemote.Service;

public interface ITenistasService
{
    const long RefreshTime = 50000;

    Task<Result<List<Tenista>, TenistaError>> GetAll(bool fromRemote);

    Task<Result<Tenista, TenistaError>> GetById(long id);

    Task<Result<Tenista, TenistaError>> Save(Tenista tenista);

    Task<Result<Tenista, TenistaError>> Update(long id, Tenista tenista);

    Task<Result<long, TenistaError>> Delete(long id);

    Task<Result<int, TenistaError>> ImportData(FileInfo file);

    Task<Result<int, TenistaError>> ExportData(FileInfo file, bool fromRemote);

    void EnableAutoRefresh();

    void DisableAutoRefresh();

    void LoadData();
}