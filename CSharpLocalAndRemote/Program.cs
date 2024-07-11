// See https://aka.ms/new-console-template for more information

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


// Eliminamos un tenista
tenistasService.DeleteAsync(2).Result.Match(
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
await Task.Delay(5000); // Esperamos un segundo para que se complete


Console.WriteLine("🔇 Desactivamos la escucha de notificaciones de tenistas 🔇");
tenistasService.DisableAutoRefresh();
notifications.Dispose();

var csvImportFile = Path.Combine("Data", "tenistas2.csv");
tenistasService.ImportDataAsync(new FileInfo(csvImportFile)).Result.Match(
    tenistas => { Console.WriteLine($"Tenistas importados desde csv: {tenistas}"); },
    error => Console.WriteLine(error)
);

var jsonImportFile = Path.Combine("Data", "tenistas3.json");
tenistasService.ImportDataAsync(new FileInfo(jsonImportFile)).Result.Match(
    tenistas => { Console.WriteLine($"Tenistas importados desde json: {tenistas}"); },
    error => Console.WriteLine(error)
);

var csvExportFile = Path.Combine("Data", "tenistas_export.csv");
tenistasService.ExportDataAsync(new FileInfo(csvExportFile), true).Result.Match(
    success => Console.WriteLine($"Tenistas exportados a csv: {success}"),
    error => Console.WriteLine(error)
);

var jsonExportFile = Path.Combine("Data", "tenistas_export.json");
tenistasService.ExportDataAsync(new FileInfo(jsonExportFile), true).Result.Match(
    success => Console.WriteLine($"Tenistas exportados a json: {success}"),
    error => Console.WriteLine(error)
);


tenistas = tenistasService.GetAllAsync(true).Result.Value;

// Comenzamos las operaciones con colecciones y LINQ
//tenistas ordenados con ranking, es decir, por puntos de mayor a menor
Console.WriteLine("Tenistas ordenados por ranking");
tenistas
    .OrderByDescending(t => t.Puntos) // Ordenamos de mayor a menor
    .Select((tenista, index) => new { Tenista = tenista, Index = index + 1 }) // Añadimos el ranking
    .ToList()
    .ForEach(item =>
        Console.WriteLine($"Ranking {item.Index}: {item.Tenista.Nombre} -> {item.Tenista.Puntos}")
    );

// Media de altura de los tenistas
var mediaAltura = tenistas.Average(t => t.Altura);
Console.WriteLine($"Media de altura de los tenistas: {mediaAltura}");

// Media de peso de los tenistas
var mediaPeso = tenistas.Average(t => t.Peso);
Console.WriteLine($"Media de peso de los tenistas: {mediaPeso}");

// Tenista más alto
var tenistaMasAlto = tenistas.OrderByDescending(t => t.Altura).First();
Console.WriteLine($"Tenista más alto: {tenistaMasAlto}");

// Tenista más bajo
var tenistaMasBajo = tenistas.OrderBy(t => t.Altura).First();
Console.WriteLine($"Tenista más bajo: {tenistaMasBajo}");

// Tenistas españoles
var tenistasEspanoles = tenistas.Where(t => t.Pais == "España").ToList();
Console.WriteLine($"Tenistas españoles: {tenistasEspanoles.Count}");

// Tenistas con más de 5000 puntos
var tenistasMasDe5000Puntos = tenistas.Where(t => t.Puntos > 5000).ToList();
Console.WriteLine($"Tenistas con más de 5000 puntos: {tenistasMasDe5000Puntos.Count}");

// Tenistas agrupados por pais
Console.WriteLine("Tenistas agrupados por pais");
var tenistasPorPais = tenistas.GroupBy(t => t.Pais).ToList();
tenistasPorPais.ForEach(group => { Console.WriteLine($"Tenistas de {group.Key}: {group.Count()}"); });

// Número de tenistas agrupados por pais y ordenados por puntos descendente
Console.WriteLine("Tenistas agrupados por pais y ordenados por puntos descendente");
var tenistasPorPaisOrdenados = tenistas
    .GroupBy(t => t.Pais)
    .ToDictionary(
        g => g.Key,
        g => g.OrderByDescending(t => t.Puntos).ToList()
    );

foreach (var (pais, tenistasDelPais) in tenistasPorPaisOrdenados)
{
    Console.WriteLine($"Tenistas de {pais}: {tenistasDelPais.Count}");
    foreach (var t in tenistasDelPais) Console.WriteLine(t);
}

// Puntuación total de los tenistas agrupados por pais
Console.WriteLine("Puntuación total de los tenistas agrupados por pais");
var puntuacionTotalPorPais = tenistas
    .GroupBy(t => t.Pais)
    .Select(g => new
    {
        Pais = g.Key,
        PuntosTotales = g.Sum(t => t.Puntos)
    });

foreach (var item in puntuacionTotalPorPais)
    Console.WriteLine($"Puntuación total de los tenistas de {item.Pais}: {item.PuntosTotales}");

// pais con puntuación total más alta (cogemos el resultado anterior)
var paisMasPuntuacion = puntuacionTotalPorPais.MaxBy(p => p.PuntosTotales);

Console.WriteLine($"País con más puntuación total: {paisMasPuntuacion.Pais} -> {paisMasPuntuacion.PuntosTotales}");

Console.WriteLine("👋👋 Adiós Tenistas! 👋👋");