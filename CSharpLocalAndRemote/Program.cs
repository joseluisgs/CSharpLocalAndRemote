// See https://aka.ms/new-console-template for more information

using System.Text;
using CSharpFunctionalExtensions;
using CSharpLocalAndRemote.Storage;

Console.OutputEncoding = Encoding.UTF8; // Necesario para mostrar emojis
Console.WriteLine("🎾🎾 Hola Tenistas! 🎾🎾");

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