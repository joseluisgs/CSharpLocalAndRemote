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

public static class HostBuilderHelper
{
    public static IHost BuildHost(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
                config.AddJsonFile("appsettings.json", true, true);
            })
            .ConfigureServices(ConfigureServices)
            .Build();

        // Se lo pasamos a AppInjector para que pueda resolver dependencias
        AppInjector.SetServiceProvider(host.Services);

        return host;
    }

    private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        RegisterSettings(context, services); // Registro de configuraciones
        RegisterDatabaseAccess(services); // Registro de acceso a base de datos
        RegisterRepositories(services); // Registro de repositorios
        RegisterCache(services); // Registro de cache
        RegisterStorage(services); // Registro de almacenamiento
        RegisterNotifications(services); // Registro de notificaciones
        RegisterTenistasService(services); // Registro de servicio de tenistas
    }

    private static void RegisterSettings(HostBuilderContext context, IServiceCollection services)
    {
        // Configuración de secciones de appsettings.json
        services.Configure<DatabaseSettings>(context.Configuration.GetSection("Database"));
        services.Configure<ApiSettings>(context.Configuration.GetSection("ApiRest"));
        services.Configure<CacheSettings>(context.Configuration.GetSection("Cache"));
        services.Configure<RefreshSettings>(context.Configuration.GetSection("Refresh"));
    }

    private static void RegisterDatabaseAccess(IServiceCollection services)
    {
        services.AddSingleton<EntityManager<TenistaEntity>>(provider =>
        {
            // Configuración de base de datos EntityManager, requerimos DatabaseSettings
            var databaseSettings = provider.GetRequiredService<IOptions<DatabaseSettings>>().Value;
            return new EntityManager<TenistaEntity>(new TenistasDbContext(
                new DbContextOptionsBuilder<TenistasDbContext>()
                    .UseSqlite("Data Source=" + databaseSettings.Name)
                    .EnableSensitiveDataLogging().Options));
        });
    }

    private static void RegisterRepositories(IServiceCollection services)
    {
        services.AddSingleton<ITenistasRepositoryLocal, TenistasRepositoryLocal>(provider =>
        {
            // Repositorio local de tenistas, requerimos EntityManager
            var entityManager = provider.GetRequiredService<EntityManager<TenistaEntity>>();
            return new TenistasRepositoryLocal(entityManager.Context);
        });

        services.AddSingleton<ITenistasRepositoryRemote>(provider =>
        {
            // Repositorio remoto de tenistas, requerimos ApiSettings
            var apiSettings = provider.GetRequiredService<IOptions<ApiSettings>>().Value;
            return new TenistasRepositoryRemote(RefitClient.CreateClient(apiSettings.Url));
        });
    }

    private static void RegisterCache(IServiceCollection services)
    {
        // Cache de tenistas, es una nueva instancia, por eso es Transient
        services.AddTransient<ITenistasCache, TenistasCache>(provider =>
        {
            // Cache de tenistas, requerimos CacheSettings
            var cacheSettings = provider.GetRequiredService<IOptions<CacheSettings>>().Value;
            return new TenistasCache(cacheSettings.Size);
        });
    }

    private static void RegisterStorage(IServiceCollection services)
    {
        // Almacenamiento de tenistas
        services.AddSingleton<ITenistasStorageCsv, TenistasStorageCsv>();
        services.AddSingleton<ITenistasStorageJson, TenistasStorageJson>();
    }

    private static void RegisterNotifications(IServiceCollection services)
    {
        services.AddSingleton<ITenistasNotifications, TenistasNotifications>();
    }

    private static void RegisterTenistasService(IServiceCollection services)
    {
        // Servicio de tenistas, con sus dependencias
        services.AddSingleton<ITenistasService, TenistasService>(provider =>
        {
            var refreshSettings = provider.GetRequiredService<IOptions<RefreshSettings>>().Value;
            var repositoryLocal = provider.GetRequiredService<ITenistasRepositoryLocal>();
            var repositoryRemote = provider.GetRequiredService<ITenistasRepositoryRemote>();
            var cache = provider.GetRequiredService<ITenistasCache>();
            var notifications = provider.GetRequiredService<ITenistasNotifications>();
            var storageCsv = provider.GetRequiredService<ITenistasStorageCsv>();
            var storageJson = provider.GetRequiredService<ITenistasStorageJson>();
            return new TenistasService(repositoryLocal, repositoryRemote, cache, storageCsv, storageJson, notifications,
                refreshSettings.Interval * 1000);
        });
    }
}