using CSharpFunctionalExtensions;
using CSharpLocalAndRemote.Cache;
using CSharpLocalAndRemote.Dto;
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
    private readonly TenistasRepositoryLocal _localRepository;
    private readonly object _lock = new();
    private readonly Serilog.Core.Logger _logger = LoggerUtils<TenistasService>.GetLogger();
    private readonly TenistasNotifications _notificationsService;
    private readonly long _refreshTime;
    private readonly TenistasRepositoryRemote _remoteRepository;
    private int _isRefreshing; // 0 = false, 1 = true


    public TenistasService(TenistasRepositoryLocal localReository, TenistasRepositoryRemote remoteRepository,
        ITenistasCache cache, TenistasStorageCsv csvStorage, TenistasStorageJson jsonStorage,
        TenistasNotifications notificationsService, long refreshTime)
    {
        _logger.Debug("Creando TenistasService");

        _localRepository = localReository;
        _remoteRepository = remoteRepository;
        _cache = cache;
        _csvStorage = csvStorage;
        _jsonStorage = jsonStorage;
        _notificationsService = notificationsService;
        _refreshTime = refreshTime;
    }

    // Propiedad para obtener el flujo de notificaciones, pero solo las que no son null
    public IObservable<Notification<TenistaDto>?> Notifications => _notificationsService.Notifications;


    public async Task<Result<List<Tenista>, TenistaError>> GetAll(bool fromRemote)
    {
        _logger.Debug("Obteniendo todos los tenistas");
        if (!fromRemote) return await _localRepository.GetAllAsync();

        return await _remoteRepository.GetAllAsync()
            .Check(async _ => await _localRepository.RemoveAllAsync())
            .Tap(async remoteTenistas =>
            {
                await _localRepository.SaveAllAsync(remoteTenistas);
                _cache.Clear();
            })
            .Bind(_ => _localRepository.GetAllAsync());
    }

    public Task<Result<Tenista, TenistaError>> GetById(long id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Tenista, TenistaError>> Save(Tenista tenista)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Tenista, TenistaError>> Update(long id, Tenista tenista)
    {
        throw new NotImplementedException();
    }

    public Task<Result<long, TenistaError>> Delete(long id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<int, TenistaError>> ImportData(FileInfo file)
    {
        throw new NotImplementedException();
    }

    public Task<Result<int, TenistaError>> ExportData(FileInfo file, bool fromRemote)
    {
        throw new NotImplementedException();
    }

    public void EnableAutoRefresh()
    {
        _logger.Debug("Habilitando auto refresco de datos");

        // Verificamos la variable atómica para asegurar que solo una tarea de refresco se ejecute
        if (Interlocked.CompareExchange(ref _isRefreshing, 1, 0) == 1)
        {
            _logger.Debug("El auto refresco ya está habilitado.");
            return;
        }

        Task.Run(async () =>
        {
            do
            {
                _logger.Debug("Refrescando datos en segundo plano");
                LoadData(); // Asegúrate de await la tarea LoadData
                await Task.Delay(TimeSpan.FromMilliseconds(_refreshTime));

                lock (_lock) // Sincronización para chequeo seguro de _isRefreshing
                {
                    if (_isRefreshing == 0) // Verificamos que no hay otra tarea de refresco en ejecución
                        break; // Salir del bucle si se ha deshabilitado el auto-refresh
                }
            } while (true);

            Interlocked.Exchange(ref _isRefreshing, 0); // Resetear la bandera
        });
    }

    public void DisableAutoRefresh()
    {
        _logger.Debug("Deshabilitando auto refresco de datos");
        // Sincronización para chequeo seguro de _isRefreshing para evitar que se produzca un deadlock
        // Esta acción eventualmente saldrá el bucle en EnableAutoRefresh
        lock (_lock)
        {
            _isRefreshing = 0;
        }
    }


    public async void LoadData()
    {
        _logger.Debug("Cargando datos");
        await _localRepository.RemoveAllAsync();
        await _remoteRepository.GetAllAsync()
            .Tap(async remoteTenistas =>
            {
                await _localRepository.SaveAllAsync(remoteTenistas);
                _cache.Clear();
            })
            .Tap(data => _notificationsService.Send(
                    new Notification<TenistaDto>(NotificationType.Refresh, null,
                        "Nuevos datos cargados " + data.Count,
                        DateTime.Now)
                )
            );
    }
}