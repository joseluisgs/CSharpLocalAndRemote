using System.ComponentModel;
using System.Text;
using CSharpLocalAndRemote.Dto;
using CSharpLocalAndRemote.model;
using CSharpLocalAndRemote.Storage;
using Newtonsoft.Json;

namespace Tests.Storage;

[TestFixture]
[TestOf(typeof(TenistasStorageJson))]
public class TenistasStorageJsonTest
{
    [SetUp]
    public void SetUp()
    {
        _storage = new TenistasStorageJson();
    }

    private TenistasStorageJson _storage;

    [Test]
    [DisplayName("Importar fichero JSON debe devolver error si el fichero no existe")]
    public async Task Importar_DeberiaDevolverErrorSiElFicheroNoExiste()
    {
        var directoryPath = Path.Combine(Path.GetTempPath(), "noimportcsv");
        var filePath = Path.Combine(directoryPath, "noExiste.csv");
        var fileInfo = new FileInfo(filePath);

        var result = await _storage.ImportAsync(fileInfo);

        Assert.Multiple(() =>
        {
            Assert.IsTrue(result.IsFailure, "El resultado debería ser un error");
            Assert.That(result.Error.ToString().Contains("ERROR: El fichero no existe"), Is.EqualTo(true),
                "El error debería ser que el fichero no existe");
        });

        if (Directory.Exists(directoryPath)) Directory.Delete(directoryPath, true);
    }

    [Test]
    [DisplayName("Importar fichero JSON debe devolver tenistas si el fichero existe")]
    public async Task Importar_DeberiaDevolverTenistasSiExisteElFichero()
    {
        var tempFilePath = Path.Combine(Path.GetTempPath(), "tenistas.json");
        var tenistas = new List<Tenista>
        {
            new("Roger Federer", "Suiza", 185, 85, 2000, Mano.Diestro, new DateTime(1981, 8, 8), id: 1)
        };


        var json = JsonConvert.SerializeObject(tenistas, Formatting.Indented);
        await File.WriteAllTextAsync(tempFilePath, json, Encoding.UTF8);

        var file = new FileInfo(tempFilePath);
        var result = await _storage.ImportAsync(file);

        Assert.Multiple(() =>
        {
            Assert.IsTrue(result.IsSuccess, "El resultado debería ser exitoso");
            Assert.That(result.Value.Count, Is.EqualTo(1), "Debería haber un tenista en la lista");
            Assert.That(result.Value[0].Nombre, Is.EqualTo("Roger Federer"),
                "El nombre del tenista debería ser correcto");
        });

        if (Directory.Exists(tempFilePath)) Directory.Delete(tempFilePath, true);
    }

    [Test]
    [DisplayName("Exportar fichero JSON debe devolver éxito si logra escribir el fichero")]
    public async Task Exportar_DeberiaDevolverExitoSiEscribeElFichero()
    {
        var tempFilePath = Path.Combine(Path.GetTempPath(), "tenistas.json");
        var file = new FileInfo(tempFilePath);
        var tenistas = new List<Tenista>
        {
            new("Novak Djokovic", "Serbia", 188, 77, 1500, Mano.Diestro, new DateTime(1987, 5, 22), id: 1)
        };

        var result = await _storage.ExportAsync(file, tenistas);

        Assert.Multiple(() =>
        {
            Assert.IsTrue(result.IsSuccess, "El resultado debería ser exitoso");
            Assert.That(result.Value, Is.EqualTo(1), "Debería haber exportado un tenista");
        });

        var json = await File.ReadAllTextAsync(tempFilePath, Encoding.UTF8);
        var tenistasDto = JsonConvert.DeserializeObject<List<TenistaDto>>(json);
        Assert.IsNotNull(tenistasDto, "La deserialización no debería devolver null");
        Assert.That(tenistasDto.Count, Is.EqualTo(1), "Debería haber un tenista en el archivo");

        if (Directory.Exists(tempFilePath)) Directory.Delete(tempFilePath, true);
    }

    [Test]
    [DisplayName("Exportar fichero JSON debe devolver error si no puede escribir el fichero")]
    public async Task Exportar_DeberiaDevolverErrorSiNoPuedeEscribirElFichero()
    {
        var directoryPath = Path.Combine(Path.GetTempPath(), "nonexportjson");
        var filePath = Path.Combine(directoryPath, "noExiste.json");
        var fileInfo = new FileInfo(filePath);

        var tenistas = new List<Tenista>
        {
            new("Roger Federer", "Suiza", 185, 85, 2000, Mano.Diestro, new DateTime(1981, 8, 8), id: 1)
        };

        // Asegúrate de que el directorio no exista
        if (Directory.Exists(directoryPath)) Directory.Delete(directoryPath, true);

        var result = await _storage.ExportAsync(fileInfo, tenistas);

        Assert.IsTrue(result.IsFailure, "El resultado debería ser un error");
        Assert.That(result.Error.ToString().Contains("ERROR: No se puede acceder al fichero"), Is.True,
            "El mensaje de error debería ser correcto");
    }
}