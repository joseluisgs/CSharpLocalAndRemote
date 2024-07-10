using CSharpFunctionalExtensions;
using CSharpLocalAndRemote.Cache;
using CSharpLocalAndRemote.Error;
using CSharpLocalAndRemote.Logger;
using CSharpLocalAndRemote.model;
using CSharpLocalAndRemote.Notification;
using CSharpLocalAndRemote.Repository;
using CSharpLocalAndRemote.Storage;

namespace CSharpLocalAndRemote.Service;

public class TenistasService : ITenistasService
{
    private readonly ITenistasCache _cache;
    private readonly TenistasStorageCsv _csvStorage;
    private readonly TenistasStorageJson _jsonStorage;
    private readonly TenistasRepositoryLocal _localReository;
    private readonly TenistasNotifications _notificationsService;
    private readonly TenistasRepositoryRemote _remoteRepository;
    private long _refreshTime;
    private readonly Serilog.Core.Logger _logger = LoggerUtils<TenistasService>.GetLogger();


    public TenistasService(TenistasRepositoryLocal localReository, TenistasRepositoryRemote remoteRepository,
        ITenistasCache cache, TenistasStorageCsv csvStorage, TenistasStorageJson jsonStorage,
        TenistasNotifications notificationsService, long refreshTime)
    {
        _localReository = localReository;
        _remoteRepository = remoteRepository;
        _cache = cache;
        _csvStorage = csvStorage;
        _jsonStorage = jsonStorage;
        _notificationsService = notificationsService;
        _refreshTime = refreshTime;
    }


    public IObservable<Result<List<Tenista>, TenistaError>> GetAll(bool fromRemote)
    {
        throw new NotImplementedException();
    }

    public IObservable<Result<Tenista, TenistaError>> GetById(long id)
    {
        throw new NotImplementedException();
    }

    public IObservable<Result<Tenista, TenistaError>> Save(Tenista tenista)
    {
        throw new NotImplementedException();
    }

    public IObservable<Result<Tenista, TenistaError>> Update(long id, Tenista tenista)
    {
        throw new NotImplementedException();
    }

    public IObservable<Result<long, TenistaError>> Delete(long id)
    {
        throw new NotImplementedException();
    }

    public IObservable<Result<int, TenistaError>> ImportData(FileInfo file)
    {
        throw new NotImplementedException();
    }

    public IObservable<Result<int, TenistaError>> ExportData(FileInfo file, bool fromRemote)
    {
        throw new NotImplementedException();
    }

    public void Refresh()
    {
        throw new NotImplementedException();
    }

    public void LoadData()
    {
        throw new NotImplementedException();
    }
}