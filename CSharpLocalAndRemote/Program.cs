﻿// See https://aka.ms/new-console-template for more information

using System.Text;
using CSharpFunctionalExtensions;
using CSharpLocalAndRemote.Cache;
using CSharpLocalAndRemote.Database;
using CSharpLocalAndRemote.Notification;
using CSharpLocalAndRemote.Repository;
using CSharpLocalAndRemote.Rest;
using CSharpLocalAndRemote.Service;
using CSharpLocalAndRemote.Storage;
using Microsoft.EntityFrameworkCore;

Console.OutputEncoding = Encoding.UTF8; // Necesario para mostrar emojis
Console.WriteLine("🎾🎾 ¡Hola Tenistas! 🎾🎾");

var manager = new EntityManager<TenistaEntity>(
    new TenistasDbContext(
        new DbContextOptionsBuilder<TenistasDbContext>()
            .UseSqlite("Data Source=tenistas.db")
            .EnableSensitiveDataLogging()
            .Options
    )
);

const string baseUrl = "https://my-json-server.typicode.com/joseluisgs/KotlinLocalAndRemote/";
var client = RefitClient.CreateClient(baseUrl);

var tenistasService = new TenistasService(
    new TenistasRepositoryLocal(manager.Context),
    new TenistasRepositoryRemote(client),
    new TenistasCache(5),
    new TenistasStorageCsv(),
    new TenistasStorageJson(),
    new TenistasNotifications(),
    5000
);

// Creamos el trabajo de notificaciones
Console.WriteLine("🔊 Escuchando notificaciones de tenistas 🔊");
var notifications = tenistasService.Notifications.Subscribe(notification =>
{
    switch (notification!.Type)
    {
        case NotificationType.Created:
            Console.WriteLine("🟢 Notificación de creación de tenista: " + notification.Message + " -> " +
                              notification.Item);
            break;
        case NotificationType.Updated:
            Console.WriteLine("🟠 Notificación de actualización de tenista: " + notification.Message + " -> " +
                              notification.Item);
            break;
        case NotificationType.Deleted:
            Console.WriteLine("🔴 Notificación de eliminación de tenista: " + notification.Message);
            break;
        case NotificationType.Refresh:
            Console.WriteLine("🔵 Notificación de refresco de tenistas: " + notification.Message);
            break;
        default:
            Console.WriteLine("🟣 Notificación desconocida: " + notification.Message);
            break;
    }
});


Console.WriteLine("🔄 Refrescamos los tenistas 🔄");

tenistasService.EnableAutoRefresh();

await Task.Delay(2000);

// Obtenemos todos los tenistas

var tenistas = tenistasService.GetAllAsync(false).Result.Match(
    tenistas =>
    {
        Console.WriteLine($"Encontrados {tenistas.Count} tenistas:");
        Console.WriteLine(string.Join("\n", tenistas));
        return tenistas;
    },
    error =>
    {
        Console.WriteLine($"Error al obtener los tenistas: {error}");
        return [];
    }
);

// Obtenemos un tenista que existe
tenistasService.GetByIdAsync(1).Result.Match(
    tenista => Console.WriteLine($"Tenista encontrado: {tenista}"),
    error => Console.WriteLine(error)
);

// Obtenemos el mismo tenista que tiene que estar en la cache
tenistasService.GetByIdAsync(1).Result.Match(
    tenista => Console.WriteLine($"Tenista encontrado: {tenista}"),
    error => Console.WriteLine(error)
);

// Obtenemos un tenista que no existe
tenistasService.GetByIdAsync(-1).Result.Match(
    tenista => Console.WriteLine($"Tenista encontrado: {tenista}"),
    error => Console.WriteLine(error)
);

// Guardamos un tenista
var tenista = tenistas[0];
tenista.Nombre = "Test save";
tenista.Pais = "KAKA";
tenistasService.SaveAsync(tenista).Result.Match(
    tenista => Console.WriteLine($"Tenista guardado: {tenista}"),
    error => Console.WriteLine(error)
);


// Guardamos un tenista con error de validación
var tenistaError = tenistas[0];
tenistaError.Nombre = "";
tenistasService.SaveAsync(tenistaError).Result.Match(
    tenista => Console.WriteLine($"Tenista guardado: {tenista}"),
    error => Console.WriteLine(error)
);

// obtenemos todos los tenistas
tenistasService.GetAllAsync(false).Result.Match(
    tenistas =>
    {
        Console.WriteLine($"Encontrados {tenistas.Count} tenistas:");
        Console.WriteLine(string.Join("\n", tenistas));
    },
    error => { Console.WriteLine($"Error al obtener los tenistas: {error}"); }
);

// Actualizamos un tenista
var tenistaToUpdate = tenistas[0];
tenistaToUpdate.Nombre = "Test Update";
tenistaToUpdate.Pais = "KAKA";
tenistasService.UpdateAsync(1, tenistaToUpdate).Result.Match(
    tenista => Console.WriteLine($"Tenista actualizado: {tenista}"),
    error => Console.WriteLine(error)
);

// Actualizamos un tenista que no existe
tenistasService.UpdateAsync(-1, tenistaToUpdate).Result.Match(
    tenista => Console.WriteLine($"Tenista actualizado: {tenista}"),
    error => Console.WriteLine(error)
);

// Obtenemos todos los tenistas
tenistasService.GetAllAsync(false).Result.Match(
    tenistas =>
    {
        Console.WriteLine($"Encontrados {tenistas.Count} tenistas:");
        Console.WriteLine(string.Join("\n", tenistas));
    },
    error => { Console.WriteLine($"Error al obtener los tenistas: {error}"); }
);

// Eliminamos un tenista
tenistasService.DeleteAsync(1).Result.Match(
    id => Console.WriteLine($"Tenista eliminado con id: {id}"),
    error => Console.WriteLine(error)
);

// Eliminamos un tenista que no existe
tenistasService.DeleteAsync(-1).Result.Match(
    id => Console.WriteLine($"Tenista eliminado con id: {id}"),
    error => Console.WriteLine(error)
);

// Obtenemos todos los tenistas
tenistasService.GetAllAsync(false).Result.Match(
    tenistas =>
    {
        Console.WriteLine($"Encontrados {tenistas.Count} tenistas:");
        Console.WriteLine(string.Join("\n", tenistas));
    },
    error => { Console.WriteLine($"Error al obtener los tenistas: {error}"); }
);


// Espe
/*await Task.Delay(30000); // Esperamos un segundo para que se complete


Console.WriteLine("🔇 Desactivamos la escucha de notificaciones de tenistas 🔇");
tenistasService.DisableAutoRefresh();
notifications.Dispose();*/


Console.WriteLine("🎾🎾 Adiós Tenistas! 🎾🎾");

/*var csvImportFile = Path.Combine("Data", "tenistas2.csv");
tenistasService.ImportDataAsync(new FileInfo(csvImportFile)).Result.Match(
    tenistas => { Console.WriteLine($"Tenistas importados desde csv: {tenistas}"); },
    error => Console.WriteLine(error)
);

var jsonImportFile = Path.Combine("Data", "tenistas3.json");
tenistasService.ImportDataAsync(new FileInfo(jsonImportFile)).Result.Match(
    tenistas => { Console.WriteLine($"Tenistas importados desde json: {tenistas}"); },
    error => Console.WriteLine(error)
);*/
/*
// Cramos las notificaciones
var notifications = new TenistasNotifications();
Console.WriteLine("Notificaciones creadas");

// Suscribirse y observar
notifications.Notifications
    .Subscribe(notification => { Console.WriteLine($"Recibida notificación: {notification}"); });

await Task.Delay(1000); // Esperamos un segundo para que se suscriba


// Ahora vamos con el almacenamiento JSON
var storageJson = new TenistasStorageJson();

var tenistas = storageJson.ImportAsync(new FileInfo("Data/tenistas.json")).Result.Value ?? [];

Console.WriteLine($"Tenistas importados: {tenistas.Count}");

await notifications.Send(new Notification<TenistaDto>(
        NotificationType.Created,
        tenistas[0].ToTenistaDto(),
        "Tenista creado",
        DateTime.Now
    )
);



// Creamos el EntityManager, que es el encargado encapsular el trabajo con la base de datos
// Para la entidad TenistaEntity (que es la representación de la tabla Tenista en la base de datos)
// y le pasamos el contexto de la base de datos, que es el encargado de la conexión con la base de datos
// que a su vez necesita las opciones de configuración de la base de datos
var manager = new EntityManager<TenistaEntity>(
    new TenistasDbContext(
        new DbContextOptionsBuilder<TenistasDbContext>()
            .UseSqlite("Data Source=tenistas.db")
            .Options
    )
);

var localRepository = new TenistasRepositoryLocal(manager.Context);

// Insertamos el tenista 0 en la base de datos

localRepository.SaveAsync(tenistas[0]).Result.Match(
    tenista => Console.WriteLine($"Tenista insertado: {tenista}"),
    error => Console.WriteLine($"Error al insertar el tenista: {error}")
);

// inserto el tenista 2 en la base de datos
localRepository.SaveAsync(tenistas[1]).Result.Match(
    tenista => Console.WriteLine($"Tenista insertado: {tenista}"),
    error => Console.WriteLine($"Error al insertar el tenista: {error}")
);

localRepository.SaveAsync(tenistas[2]).Result.Match(
    tenista => Console.WriteLine($"Tenista insertado: {tenista}"),
    error => Console.WriteLine($"Error al insertar el tenista: {error}")
);

// Obtenemos todos los tenistas de la base de datos
localRepository.GetAllAsync().Result.Match(
    tenistas =>
    {
        Console.WriteLine($"Encontrados {tenistas.Count} tenistas en la base de datos:");
        tenistas.ForEach(Console.WriteLine);
    },
    error => Console.WriteLine($"Error al obtener los tenistas: {error}")
);

// Obtenemos el tenista con id 1
localRepository.GetByIdAsync(1).Result.Match(
    tenista => Console.WriteLine($"Tenista encontrado: {tenista}"),
    error => Console.WriteLine($"Error al obtener el tenista: {error}")
);

// Obtenemos el tenista que no existe
localRepository.GetByIdAsync(-1).Result.Match(
    tenista => Console.WriteLine($"Tenista encontrado: {tenista}"),
    error => Console.WriteLine($"Error al obtener el tenista: {error}")
);


// Actualizamos el tenista con id 1
var tenistaToUpdate = tenistas[0];
tenistaToUpdate.Nombre = "Test Update";
localRepository.UpdateAsync(1, tenistaToUpdate).Result.Match(
    tenista => Console.WriteLine($"Tenista actualizado: {tenista}"),
    error => Console.WriteLine($"Error al actualizar el tenista: {error}")
);

// Actualizamos un tenista que no existe
localRepository.UpdateAsync(-1, tenistaToUpdate).Result.Match(
    tenista => Console.WriteLine($"Tenista actualizado: {tenista}"),
    error => Console.WriteLine($"Error al actualizar el tenista: {error}")
);

// Obtenemos todos los tenistas de la base de datos
localRepository.GetAllAsync().Result.Match(
    tenistas =>
    {
        Console.WriteLine($"Encontrados {tenistas.Count} tenistas en la base de datos:");
        tenistas.ForEach(Console.WriteLine);
    },
    error => Console.WriteLine($"Error al obtener los tenistas: {error}")
);

// Eliminamos el tenista con id 1
localRepository.DeleteAsync(1).Result.Match(
    id => Console.WriteLine($"Tenista eliminado con id: {id}"),
    error => Console.WriteLine($"Error al eliminar el tenista: {error}")
);

// Borramos un tenista que no existe
localRepository.DeleteAsync(-1).Result.Match(
    id => Console.WriteLine($"Tenista eliminado con id: {id}"),
    error => Console.WriteLine($"Error al eliminar el tenista: {error}")
);

// Obtenemos todos los tenistas de la base de datos
localRepository.GetAllAsync().Result.Match(
    tenistas =>
    {
        Console.WriteLine($"Encontrados {tenistas.Count} tenistas en la base de datos:");
        tenistas.ForEach(Console.WriteLine);
    },
    error => Console.WriteLine($"Error al obtener los tenistas: {error}")
);

tenistas.ForEach(async tenista => localRepository.SaveAsync(tenista));

await notifications.Send(new Notification<TenistaDto>(
        NotificationType.Refresh,
        null,
        "Tenistas Refrescados",
        DateTime.Now
    )
);

// Obtenemos todos los tenistas de la base de datos
localRepository.GetAllAsync().Result.Match(
    tenistas =>
    {
        Console.WriteLine($"Encontrados {tenistas.Count} tenistas en la base de datos:");
        tenistas.ForEach(Console.WriteLine);
    },
    error => Console.WriteLine($"Error al obtener los tenistas: {error}")
);

const string baseUrl = "https://my-json-server.typicode.com/joseluisgs/KotlinLocalAndRemote/";
var client = RefitClient.CreateClient(baseUrl);

var remoteRepository = new TenistasRepositoryRemote(client);

// Obtenemos todos los tenistas remotos
remoteRepository.GetAllAsync().Result.Match(
    tenistas =>
    {
        Console.WriteLine($"Encontrados {tenistas.Count} tenistas remotos:");
        tenistas.ForEach(Console.WriteLine);
    },
    error => Console.WriteLine($"Error al obtener los tenistas remotos: {error}")
);

// Obtenemos el tenista remoto con id 1
remoteRepository.GetByIdAsync(1).Result.Match(
    tenista => Console.WriteLine($"Tenista remoto encontrado: {tenista}"),
    error => Console.WriteLine($"Error al obtener el tenista remoto: {error}")
);

// Obtenemos el tenista remoto que no existe
remoteRepository.GetByIdAsync(-1).Result.Match(
    tenista => Console.WriteLine($"Tenista remoto encontrado: {tenista}"),
    error => Console.WriteLine($"Error al obtener el tenista remoto: {error}")
);

// Salvamos un tenista a remoto
remoteRepository.SaveAsync(tenistas[0]).Result.Match(
    tenista => Console.WriteLine($"Tenista remoto guardado: {tenista}"),
    error => Console.WriteLine($"Error al guardar el tenista remoto: {error}")
);

// Actualizamos un tenista a remoto
remoteRepository.UpdateAsync(1, tenistas[1]).Result.Match(
    tenista => Console.WriteLine($"Tenista remoto actualizado: {tenista}"),
    error => Console.WriteLine($"Error al actualizado el tenista remoto: {error}")
);

// Actualizamos un tenista que no existe a remoto
remoteRepository.UpdateAsync(-1, tenistas[1]).Result.Match(
    tenista => Console.WriteLine($"Tenista remoto actualizado: {tenista}"),
    error => Console.WriteLine($"Error al actualizado el tenista remoto: {error}")
);

// Eliminamos un tenista a remoto
remoteRepository.DeleteAsync(1).Result.Match(
    id => Console.WriteLine($"Tenista remoto eliminado con id: {id}"),
    error => Console.WriteLine($"Error al eliminar el tenista remoto: {error}")
);

// Eliminamos un tenista que no existe a remoto
remoteRepository.DeleteAsync(-1).Result.Match(
    id => Console.WriteLine($"Tenista remoto eliminado con id: {id}"),
    error => Console.WriteLine($"Error al eliminar el tenista remoto: {error}")
);

// Voy a leer un csv
new TenistasStorageCsv().ImportAsync(new FileInfo("Data/tenistas.csv")).Result.Match(
    tenistas =>
    {
        Console.WriteLine($"Tenistas importados: {tenistas.Count}");
        tenistas.ForEach(Console.WriteLine);
    },
    error => Console.WriteLine($"Error al importar los tenistas: {error}")
);

// Escritura de un csv
new TenistasStorageCsv().ExportAsync(new FileInfo("Data/tenistas_export.csv"), tenistas).Wait();


notifications.Stop(); // Detener las notificaciones*/