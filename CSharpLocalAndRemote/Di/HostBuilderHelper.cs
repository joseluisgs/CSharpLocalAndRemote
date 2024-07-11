using CSharpLocalAndRemote.Cache;
using CSharpLocalAndRemote.Database;
using CSharpLocalAndRemote.Notification;
using CSharpLocalAndRemote.Repository;
using CSharpLocalAndRemote.Rest;
using CSharpLocalAndRemote.Service;
using CSharpLocalAndRemote.Settings;
using CSharpLocalAndRemote.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace CSharpLocalAndRemote.Di;

/**
 * Clase de ayuda para construir el host de la aplicación.
 * El host se configura con los servicios necesarios para la aplicación.
 * Se prepara la configuración de la aplicación y se registran los servicios necesarios.
 * La inyección de dependencias se realiza en esta clase.
 * Se puede leer la configuración y registrar los servicios utilizando el método ConfigureServices.
 * Configurar el log y el manejo de excepciones utilizando el método ConfigureLogging.
 */
public static class HostBuilderHelper
{
    public static IHost BuildHost(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
                config.AddJsonFile("appsettings.json", true, true);
            })
            .ConfigureServices((context, services) =>
            {
                // Settings dependencias usando las secciones del archivo appsettings.json
                services.Configure<DatabaseSettings>(context.Configuration.GetSection("Database"));
                services.Configure<ApiSettings>(context.Configuration.GetSection("ApiRest"));
                services.Configure<CacheSettings>(context.Configuration.GetSection("Cache"));
                services.Configure<RefreshSettings>(context.Configuration.GetSection("Refresh"));

                // Configurar el acceso a la base de datos
                services.AddSingleton<EntityManager<TenistaEntity>>(provider =>
                {
                    var databaseSettings = provider.GetRequiredService<IOptions<DatabaseSettings>>().Value;

                    return new EntityManager<TenistaEntity>(
                        new TenistasDbContext(
                            new DbContextOptionsBuilder<TenistasDbContext>()
                                .UseSqlite("Data Source=" + databaseSettings.Name)
                                .EnableSensitiveDataLogging()
                                .Options
                        )
                    );
                });

                // Registrar repositorios
                services.AddSingleton<ITenistasRepositoryLocal, TenistasRepositoryLocal>(provider =>
                {
                    var entityManager = provider.GetRequiredService<EntityManager<TenistaEntity>>();
                    return new TenistasRepositoryLocal(entityManager.Context);
                });

                // Registrar Repositorio remoto, tiene una dependencia de ApiSettings
                services.AddSingleton<ITenistasRepositoryRemote>(provider =>
                {
                    var apiSettings = provider.GetRequiredService<IOptions<ApiSettings>>().Value;
                    return new TenistasRepositoryRemote(RefitClient.CreateClient(apiSettings.Url));
                });

                // Registrar almacenamientos
                // Cache
                services.AddSingleton<ITenistasCache, TenistasCache>(provider =>
                {
                    var cacheSettings = provider.GetRequiredService<IOptions<CacheSettings>>().Value;
                    return new TenistasCache(cacheSettings.Size);
                });

                // Almacenamiento en ficheros
                services.AddSingleton<ITenistasStorageCsv, TenistasStorageCsv>();
                services.AddSingleton<ITenistasStorageJson, TenistasStorageJson>();

                // Registrar notificaciones
                services.AddSingleton<ITenistasNotifications, TenistasNotifications>();

                // Registrar servicios
                services.AddSingleton<ITenistasService, TenistasService>(provider =>
                {
                    var refreshSettings = provider.GetRequiredService<IOptions<RefreshSettings>>().Value;
                    var repositoryLocal = provider.GetRequiredService<ITenistasRepositoryLocal>();
                    var repositoryRemote = provider.GetRequiredService<ITenistasRepositoryRemote>();
                    var cache = provider.GetRequiredService<ITenistasCache>();
                    var notifications = provider.GetRequiredService<ITenistasNotifications>();
                    var storageCsv = provider.GetRequiredService<ITenistasStorageCsv>();
                    var storageJson = provider.GetRequiredService<ITenistasStorageJson>();
                    return new TenistasService(
                        repositoryLocal,
                        repositoryRemote,
                        cache,
                        storageCsv,
                        storageJson,
                        notifications,
                        refreshSettings.Interval * 1000 // Convertir a milisegundos
                    );
                });
            })
            .Build();
    }
}