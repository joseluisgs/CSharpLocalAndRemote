﻿using CSharpFunctionalExtensions;
using CSharpLocalAndRemote.Cache;
using CSharpLocalAndRemote.Dto;
using CSharpLocalAndRemote.Error;
using CSharpLocalAndRemote.Logger;
using CSharpLocalAndRemote.Mapper;
using CSharpLocalAndRemote.model;
using CSharpLocalAndRemote.Notification;
using CSharpLocalAndRemote.Repository;
using CSharpLocalAndRemote.Storage;
using CSharpLocalAndRemote.Validator;

namespace CSharpLocalAndRemote.Service;

public class TenistasService : ITenistasService
{
    private readonly ITenistasCache _cache;
    private readonly ITenistasStorageCsv _csvStorage;
    private readonly ITenistasStorageJson _jsonStorage;
    private readonly ITenistasRepositoryLocal _localRepository;
    private readonly object _lock = new();
    private readonly Serilog.Core.Logger _logger = LoggerUtils<TenistasService>.GetLogger();
    private readonly ITenistasNotifications _notificationsService;
    private readonly long _refreshTime;
    private readonly ITenistasRepositoryRemote _remoteRepository;
    private int _isRefreshing; // 0 = false, 1 = true


    public TenistasService(ITenistasRepositoryLocal localReository, ITenistasRepositoryRemote remoteRepository,
        ITenistasCache cache, ITenistasStorageCsv csvStorage, ITenistasStorageJson jsonStorage,
        ITenistasNotifications notificationsService, long refreshTime)
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


    public async Task<Result<List<Tenista>, TenistaError>> GetAllAsync(bool fromRemote)
    {
        _logger.Debug("Obteniendo todos los tenistas");
        if (!fromRemote) return await _localRepository.GetAllAsync();

        return await _remoteRepository.GetAllAsync()
            .Check(async _ => await _localRepository.RemoveAllAsync())
            .Check(async remoteTenistas => await _localRepository.SaveAllAsync(remoteTenistas))
            .Check(_ => _localRepository.GetAllAsync())
            .Tap(_ => _cache.Clear());
    }

    public async Task<Result<Tenista, TenistaError>> GetByIdAsync(long id)
    {
        _logger.Debug("Obteniendo tenista por id: " + id);

        // Buscar en la caché
        var cachedTenista = _cache.Get(id);
        if (cachedTenista is not null)
            // Si se encuentra en la caché, devolver el resultado exitoso
            return Result.Success<Tenista, TenistaError>(cachedTenista);

        // Buscar en el repositorio local y mete en cache y si no se encuentra, buscar en el repositorio remoto
        // Si quieres puedes escribirlo con if - else - if - else
        return await _localRepository.GetByIdAsync(id)
            .Tap(localTenista => _cache.Put(id, localTenista))
            .OnFailureCompensate(async () =>
            {
                return await _remoteRepository.GetByIdAsync(id)
                    .Check(async tenistaRemote => await _localRepository.SaveAsync(tenistaRemote))
                    .Tap(localTenista => _cache.Put(id, localTenista));
            });
    }

    public async Task<Result<Tenista, TenistaError>> SaveAsync(Tenista tenista)
    {
        _logger.Debug("Guardando tenista: " + tenista);
        // Validamos el tenista antes de guardarlo remetoto
        var validationResult = tenista.Validate();
        if (validationResult.IsFailure)
            return Result.Failure<Tenista, TenistaError>(validationResult.Error);

        // Guardamos el tenista en el repositorio remoto, local cache y lanzamos notificación
        return await _remoteRepository.SaveAsync(tenista)
            .Check(async tenistaRemoted => await _localRepository.SaveAsync(tenistaRemoted))
            .Tap(tenistaSaved => _cache.Put(tenistaSaved.Id, tenistaSaved))
            .Tap(tenistaSaved => _notificationsService.Send(
                new Notification<TenistaDto>(NotificationType.Created, tenistaSaved.ToTenistaDto(),
                    "Tenista guardado con id: " + tenistaSaved.Id,
                    DateTime.Now)
            ));
    }

    public async Task<Result<Tenista, TenistaError>> UpdateAsync(long id, Tenista tenista)
    {
        _logger.Debug("Actualizando tenista con id: " + id);

        // Validamos el tenista antes de actualizarlo
        var validationResult = tenista.Validate();
        if (validationResult.IsFailure)
            return Result.Failure<Tenista, TenistaError>(validationResult.Error);

        // Si existe actualizamos en remoto, local y cache
        return await GetByIdAsync(id)
            .Check(async _ => await _remoteRepository.UpdateAsync(id, tenista))
            .Check(async _ => await _localRepository.UpdateAsync(id, tenista))
            .Tap(data => _notificationsService.Send(
                new Notification<TenistaDto>(NotificationType.Updated, tenista.ToTenistaDto(),
                    "Tenista actualizado con id: " + tenista.Id,
                    DateTime.Now)
            ));
    }

    public async Task<Result<long, TenistaError>> DeleteAsync(long id)
    {
        _logger.Debug("Eliminando tenista con id: " + id);

        return await GetByIdAsync(id)
            .Check(async _ => await _remoteRepository.DeleteAsync(id))
            .Check(async _ => await _localRepository.DeleteAsync(id))
            .Tap(_ => _cache.Remove(id))
            .Tap(_ => _notificationsService.Send(
                new Notification<TenistaDto>(NotificationType.Deleted, null,
                    "Tenista eliminado con id: " + id,
                    DateTime.Now)
            ))
            .Map(_ => id);
    }

    public async Task<Result<int, TenistaError>> ImportDataAsync(FileInfo file)
    {
        _logger.Debug("Importando datos desde archivo: " + file.FullName);
        return file.Extension.ToLower() switch
        {
            ".csv" => await ImportCsvAsync(file),
            ".json" => await ImportJsonAsync(file),
            _ => Result.Failure<int, TenistaError>(
                new TenistaError.StorageError("Formato de archivo no soportado: " + file.Extension))
        };
    }

    public Task<Result<int, TenistaError>> ExportDataAsync(FileInfo file, bool fromRemote)
    {
        _logger.Debug("Exportando datos a archivo: " + file.FullName);
        return file.Extension.ToLower() switch
        {
            ".csv" => ExportCsvAsync(file, fromRemote),
            ".json" => ExportJsonAsync(file, fromRemote),
            _ => Task.FromResult(Result.Failure<int, TenistaError>(
                new TenistaError.StorageError("Formato de archivo no soportado: " + file.Extension)))
        };
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
                await LoadDataAsync(); // Asegúrate de await la tarea LoadData
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


    public async Task LoadDataAsync()
    {
        _logger.Debug("Cargando datos");
        await _remoteRepository.GetAllAsync()
            .Check(async _ => await _localRepository.RemoveAllAsync())
            .Check(async remoteTenistas => await _localRepository.SaveAllAsync(remoteTenistas))
            .Tap(_ => _cache.Clear())
            .Tap(data => _notificationsService.Send(
                    new Notification<TenistaDto>(NotificationType.Refresh, null,
                        "Nuevos datos cargados " + data.Count,
                        DateTime.Now)
                )
            );
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

    private async Task<Result<int, TenistaError>> ExportJsonAsync(FileInfo file, bool fromRemote)
    {
        _logger.Debug("Exportando datos a archivo json: " + file.FullName);
        var resultData = fromRemote
            ? await _remoteRepository.GetAllAsync()
            : await _localRepository.GetAllAsync();

        if (resultData.IsFailure) return Result.Failure<int, TenistaError>(resultData.Error);

        var res = await _jsonStorage.ExportAsync(file, resultData.Value);
        return res.IsSuccess
            ? Result.Success<int, TenistaError>(res.Value)
            : Result.Failure<int, TenistaError>(res.Error);
    }

    private async Task<Result<int, TenistaError>> ExportCsvAsync(FileInfo file, bool fromRemote)
    {
        _logger.Debug("Exportando datos a archivo csv: " + file.FullName);
        var resultData = fromRemote
            ? await _remoteRepository.GetAllAsync()
            : await _localRepository.GetAllAsync();

        if (resultData.IsFailure) return Result.Failure<int, TenistaError>(resultData.Error);

        var res = await _csvStorage.ExportAsync(file, resultData.Value);
        return res.IsSuccess
            ? Result.Success<int, TenistaError>(res.Value)
            : Result.Failure<int, TenistaError>(res.Error);
    }

    private async Task<Result<int, TenistaError>> ImportJsonAsync(FileInfo file)
    {
        _logger.Debug("Importando datos desde archivo json: " + file.FullName);

        var tenistas = await _jsonStorage.ImportAsync(file);
        return tenistas.IsSuccess
            ? await SaveAllAsync(tenistas.Value)
            : Result.Failure<int, TenistaError>(tenistas.Error);
    }

    private async Task<Result<int, TenistaError>> ImportCsvAsync(FileInfo file)
    {
        _logger.Debug("Importando datos desde archivo csv: {FileName}", file.FullName);

        var tenistas = await _csvStorage.ImportAsync(file);
        return tenistas.IsSuccess
            ? await SaveAllAsync(tenistas.Value)
            : Result.Failure<int, TenistaError>(tenistas.Error);
    }

    private async Task<Result<int, TenistaError>> SaveAllAsync(List<Tenista> tenistas)
    {
        _logger.Debug("Guardando todos los tenistas importados");
        var contador = 0;
        return await _localRepository.RemoveAllAsync()
            .Tap(async () =>
            {
                foreach (var tenista in tenistas)
                    await _remoteRepository.SaveAsync(tenista)
                        .Check(async tenistaSaved => await _localRepository.SaveAsync(tenistaSaved))
                        .Tap(_ => contador++);
            }).Map(_ => contador);
    }
}