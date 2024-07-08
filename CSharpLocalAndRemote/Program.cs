// See https://aka.ms/new-console-template for more information

using System.Text;
using CSharpFunctionalExtensions;
using CSharpLocalAndRemote.Database;
using CSharpLocalAndRemote.Mapper;
using CSharpLocalAndRemote.Storage;
using Microsoft.EntityFrameworkCore;

Console.OutputEncoding = Encoding.UTF8; // Necesario para mostrar emojis
Console.WriteLine("🎾🎾 Hola Tenistas! 🎾🎾");

var optionsBuilder = new DbContextOptionsBuilder<DbContext>();
optionsBuilder.UseSqlite("Data Source=tenistas.db");

var manager = new EntityManager<TenistaEntity>(new TenistasDbContext(optionsBuilder.Options));
// Creamos la base de datos
await manager.Context.Database.EnsureCreatedAsync();
await manager.Context.SaveChangesAsync();
await manager.Context.RemoveAllAsync();

var tenista = new TenistaEntity
{
    Nombre = "Rafa Nadal", Pais = "España", Altura = 185, Peso = 85, Puntos = 10000, Mano = "Diestro",
    FechaNacimiento = "1986-06-03", CreatedAt = "2021-10-01", UpdatedAt = "2021-10-01", IsDeleted = false
};

// Añadimos un tenista
manager.DbSet.Add(tenista);
await manager.Context.SaveChangesAsync();

Console.WriteLine($"Tenista añadido: {tenista.Id} - {tenista.Nombre}");

var tenista2 = new TenistaEntity
{
    Nombre = "Roger Federer", Pais = "Suiza", Altura = 185, Peso = 85, Puntos = 9000, Mano = "Diestro",
    FechaNacimiento = "1981-08-08", CreatedAt = "2021-10-01", UpdatedAt = "2021-10-01", IsDeleted = false
};


manager.DbSet.Add(tenista2);
await manager.Context.SaveChangesAsync();

Console.WriteLine($"Tenista añadido: {tenista2.Id} - {tenista2.Nombre}");


// Selección de los tenistas
var res = await manager.DbSet.ToListAsync();

Console.WriteLine($"Número de tenistas: {res.Count}");
res.ForEach(t => Console.WriteLine($"{t.Id} - {t.Nombre}"));

var storageCsv = new TenistasStorageCsv();

var tenistasResult = await storageCsv.ImportAsync(new FileInfo("Data/tenistas.csv"));

tenistasResult.Match(
    ok => Console.WriteLine($"Importado {ok.Count} tenistas"),
    error => Console.WriteLine(error)
);

var noExiste = await storageCsv.ImportAsync(new FileInfo("Data/tenistas_no_existe.csv"));

noExiste.Match(
    ok => Console.WriteLine($"Importado {ok.Count} tenistas"),
    error => Console.WriteLine(error)
);

// Escribimos los tenistas importados
await storageCsv.ExportAsync(new FileInfo("Data/tenistas_exportados.csv"), tenistasResult.Value).Match(
    ok => Console.WriteLine($"Exportados {ok} tenistas"),
    error => Console.WriteLine(error)
);

// Escribimos los tenistas importados en un fichero que no se puede crear
await storageCsv.ExportAsync(new FileInfo("KASKASKAS/tenistas_exportados_no_crear.csv"), tenistasResult.Value).Match(
    ok => Console.WriteLine($"Exportados {ok} tenistas"),
    error => Console.WriteLine(error)
);


// Ahora vamos con el almacenamiento JSON
var storageJson = new TenistasStorageJson();

var tenistasJsonResult = await storageJson.ImportAsync(new FileInfo("Data/tenistas.json"));

tenistasJsonResult.Match(
    ok => Console.WriteLine($"Importado {ok.Count} tenistas"),
    error => Console.WriteLine(error)
);

// no existe
var noExisteJson = await storageJson.ImportAsync(new FileInfo("Data/tenistas_no_existe.json"));

noExisteJson.Match(
    ok => Console.WriteLine($"Importado {ok.Count} tenistas"),
    error => Console.WriteLine(error)
);

// Escribimos los tenistas importados
await storageJson.ExportAsync(new FileInfo("Data/tenistas_exportados.json"), tenistasJsonResult.Value).Match(
    ok => Console.WriteLine($"Exportados {ok} tenistas"),
    error => Console.WriteLine(error)
);

// Escribimos los tenistas importados en un fichero que no se puede crear
await storageJson.ExportAsync(new FileInfo("KASKASKAS/tenistas_exportados_no_crear.json"), tenistasJsonResult.Value)
    .Match(
        ok => Console.WriteLine($"Exportados {ok} tenistas"),
        error => Console.WriteLine(error)
    );