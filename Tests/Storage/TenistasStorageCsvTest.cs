using System.ComponentModel;
using CSharpLocalAndRemote.model;
using CSharpLocalAndRemote.Storage;

namespace Tests.Storage;

[TestFixture]
[TestOf(typeof(TenistasStorageCsv))]
public class TenistasStorageCsvTest
{
    [SetUp]
    public void SetUp()
    {
        _storage = new TenistasStorageCsv();
    }

    private TenistasStorageCsv _storage;

    [Test]
    [DisplayName("Import debe devolver error si el fichero no existe")]
    public async Task Importar_DeberiaDevolverErrorSiElFicheroNoExiste()
    {
        var directoryPath = Path.Combine(Path.GetTempPath(), "nonExistingDirectory");
        var filePath = Path.Combine(directoryPath, "noExiste.csv");
        var fileInfo = new FileInfo(filePath);


        if (Directory.Exists(directoryPath)) Directory.Delete(directoryPath, true);

        var result = await _storage.ImportAsync(fileInfo);
        
        Console.WriteLine(result.Error);

        Assert.Multiple(() =>
        {
            Assert.IsTrue(result.IsFailure, "El resultado debería ser un error");
            Assert.That(result.Error.ToString().Contains("ERROR: El fichero no existe"), Is.True,
                "El mensaje de error debería ser correcto");
        });
    }

    [Test]
    [DisplayName("Importar fichero debe devolver tenistas si existe el fichero")]
    public async Task ImportarFichero_DeberiaDevolverTenistasSiExisteElFichero()
    {
        var tempFilePath = Path.Combine(Path.GetTempPath(), "tenistas.csv");
        await File.WriteAllTextAsync(tempFilePath,
            "id,nombre,pais,altura,peso,puntos,mano,fechaNacimiento,createdAt,updatedAt,deletedAt,isDeleted\n1,Rafael Nadal,España,185,85,1000,ZURDO,1986-06-03\n");

        var file = new FileInfo(tempFilePath);
        var result = await _storage.ImportAsync(file);

        Assert.Multiple(() =>
        {
            Assert.IsTrue(result.IsSuccess, "El resultado debería ser exitoso");
            Assert.That(result.Value.Count, Is.EqualTo(1), "Debería haber un tenista en la lista");
            Assert.That(result.Value[0].Nombre, Is.EqualTo("Rafael Nadal"),
                "El nombre del tenista debería ser correcto");
            Assert.That(result.Value[0].Altura, Is.EqualTo(185), "La altura del tenista debería ser correcta");
            Assert.That(result.Value[0].Peso, Is.EqualTo(85), "El peso del tenista debería ser correcto");
        });

        File.Delete(tempFilePath); // Limpieza del archivo temporal
    }

    [Test]
    [DisplayName("Exportar fichero debe devolver tenistas si escribe en el fichero")]
    public async Task Exportar_DeberiaDevolverTenistasSiEscribeEnElFichero()
    {
        var tempFilePath = Path.Combine(Path.GetTempPath(), "tenistasExport.csv");
        var file = new FileInfo(tempFilePath);
        var tenistas = new List<Tenista>
        {
            new("Roger Federer", "Suiza", 185, 85, 2000, Mano.DIESTRO, new DateTime(1981, 8, 8), id: 1)
        };

        var result = await _storage.ExportAsync(file, tenistas);

        Assert.Multiple(() =>
        {
            Assert.IsTrue(result.IsSuccess, "El resultado debería ser exitoso");
            Assert.That(result.Value, Is.EqualTo(1), "Debería haber exportado un tenista");
        });

        File.Delete(tempFilePath); // Limpieza del archivo temporal
    }

    [Test]
    [DisplayName("Exportar fichero debe devolver error si el fichero o directorio no existe")]
    public async Task Exportar_DeberiaDevolverErrorSiElFicheroNoExiste()
    {
        var directoryPath = Path.Combine(Path.GetTempPath(), "nonExistingDirectory");
        var filePath = Path.Combine(directoryPath, "noExiste.csv");
        var fileInfo = new FileInfo(filePath);

        var tenistas = new List<Tenista>
        {
            new("Roger Federer", "Suiza", 185, 85, 2000, Mano.DIESTRO, new DateTime(1981, 8, 8), id: 1)
        };

        // Asegúrate de que el directorio no exista
        if (Directory.Exists(directoryPath)) Directory.Delete(directoryPath, true);

        var result = await _storage.ExportAsync(fileInfo, tenistas);

        Assert.IsTrue(result.IsFailure, "El resultado debería ser un error");
        Assert.That(result.Error.ToString().Contains("ERROR: No se puede acceder al fichero"), Is.True,
            "El mensaje de error debería ser correcto");
    }
}