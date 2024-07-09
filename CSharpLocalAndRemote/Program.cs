﻿// See https://aka.ms/new-console-template for more information

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

var repository = new TenistasRepositoryLocal(manager.Context);

// Insertamos el tenista 0 en la base de datos

repository.SaveAsync(tenistas[0]).Result.Match(
    tenista => Console.WriteLine($"Tenista insertado: {tenista}"),
    error => Console.WriteLine($"Error al insertar el tenista: {error}")
);

// inserto el tenista 2 en la base de datos
repository.SaveAsync(tenistas[1]).Result.Match(
    tenista => Console.WriteLine($"Tenista insertado: {tenista}"),
    error => Console.WriteLine($"Error al insertar el tenista: {error}")
);

repository.SaveAsync(tenistas[2]).Result.Match(
    tenista => Console.WriteLine($"Tenista insertado: {tenista}"),
    error => Console.WriteLine($"Error al insertar el tenista: {error}")
);

// Obtenemos todos los tenistas de la base de datos
repository.GetAllAsync().Result.Match(
    tenistas =>
    {
        Console.WriteLine($"Encontrados {tenistas.Count} tenistas en la base de datos:");
        tenistas.ForEach(Console.WriteLine);
    },
    error => Console.WriteLine($"Error al obtener los tenistas: {error}")
);

// Obtenemos el tenista con id 1
repository.GetByIdAsync(1).Result.Match(
    tenista => Console.WriteLine($"Tenista encontrado: {tenista}"),
    error => Console.WriteLine($"Error al obtener el tenista: {error}")
);

// Obtenemos el tenista que no existe
repository.GetByIdAsync(-1).Result.Match(
    tenista => Console.WriteLine($"Tenista encontrado: {tenista}"),
    error => Console.WriteLine($"Error al obtener el tenista: {error}")
);


// Actualizamos el tenista con id 1
var tenistaToUpdate = tenistas[0];
tenistaToUpdate.Nombre = "Test Update";
repository.UpdateAsync(1, tenistaToUpdate).Result.Match(
    tenista => Console.WriteLine($"Tenista actualizado: {tenista}"),
    error => Console.WriteLine($"Error al actualizar el tenista: {error}")
);

// Actualizamos un tenista que no existe
repository.UpdateAsync(-1, tenistaToUpdate).Result.Match(
    tenista => Console.WriteLine($"Tenista actualizado: {tenista}"),
    error => Console.WriteLine($"Error al actualizar el tenista: {error}")
);

// Obtenemos todos los tenistas de la base de datos
repository.GetAllAsync().Result.Match(
    tenistas =>
    {
        Console.WriteLine($"Encontrados {tenistas.Count} tenistas en la base de datos:");
        tenistas.ForEach(Console.WriteLine);
    },
    error => Console.WriteLine($"Error al obtener los tenistas: {error}")
);

// Eliminamos el tenista con id 1
repository.DeleteAsync(1).Result.Match(
    id => Console.WriteLine($"Tenista eliminado con id: {id}"),
    error => Console.WriteLine($"Error al eliminar el tenista: {error}")
);

// Borramos un tenista que no existe
repository.DeleteAsync(-1).Result.Match(
    id => Console.WriteLine($"Tenista eliminado con id: {id}"),
    error => Console.WriteLine($"Error al eliminar el tenista: {error}")
);

// Obtenemos todos los tenistas de la base de datos
repository.GetAllAsync().Result.Match(
    tenistas =>
    {
        Console.WriteLine($"Encontrados {tenistas.Count} tenistas en la base de datos:");
        tenistas.ForEach(Console.WriteLine);
    },
    error => Console.WriteLine($"Error al obtener los tenistas: {error}")
);

tenistas.ForEach(async tenista => repository.SaveAsync(tenista));

// Obtenemos todos los tenistas de la base de datos
repository.GetAllAsync().Result.Match(
    tenistas =>
    {
        Console.WriteLine($"Encontrados {tenistas.Count} tenistas en la base de datos:");
        tenistas.ForEach(Console.WriteLine);
    },
    error => Console.WriteLine($"Error al obtener los tenistas: {error}")
);

// Exportamos los tenistas a un fichero JSON
storageJson.ExportAsync(new FileInfo("Data/tenistas_export.json"), tenistas).Result.Match(
    count => Console.WriteLine($"Tenistas exportados: {count}"),
    error => Console.WriteLine($"Error al exportar los tenistas: {error}")
);

const string baseUrl = "https://my-json-server.typicode.com/joseluisgs/KotlinLocalAndRemote/";
var client = RefitClient.CreateClient(baseUrl);
try
{
    var lista = await client.GetAll();
    Console.WriteLine("Lista de tenistas:");
    foreach (var tenista in lista) Console.WriteLine($"ID: {tenista.Id}, Nombre: {tenista.Nombre}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

// Ahora un tenista con ID 1
try
{
    var tenista = await client.GetById(1);
    Console.WriteLine($"Tenista con ID 1: {tenista}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}