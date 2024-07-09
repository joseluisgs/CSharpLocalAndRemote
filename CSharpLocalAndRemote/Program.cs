// See https://aka.ms/new-console-template for more information

using System.Text;
using CSharpFunctionalExtensions;
using CSharpLocalAndRemote.Database;
using CSharpLocalAndRemote.Repository;
using CSharpLocalAndRemote.Rest;
using CSharpLocalAndRemote.Storage;
using Microsoft.EntityFrameworkCore;

Console.OutputEncoding = Encoding.UTF8; // Necesario para mostrar emojis
Console.WriteLine("🎾🎾 Hola Tenistas! 🎾🎾");

// Ahora vamos con el almacenamiento JSON
var storageJson = new TenistasStorageJson();

var tenistas = storageJson.ImportAsync(new FileInfo("Data/tenistas.json")).Result.Value ?? [];

Console.WriteLine($"Tenistas importados: {tenistas.Count}");


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