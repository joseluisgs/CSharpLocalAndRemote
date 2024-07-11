using System.ComponentModel;
using CSharpFunctionalExtensions;
using CSharpLocalAndRemote.Cache;
using CSharpLocalAndRemote.Dto;
using CSharpLocalAndRemote.Error;
using CSharpLocalAndRemote.model;
using CSharpLocalAndRemote.Notification;
using CSharpLocalAndRemote.Repository;
using CSharpLocalAndRemote.Service;
using CSharpLocalAndRemote.Storage;
using Moq;

namespace Tests.Service;

[TestFixture]
[TestOf(typeof(TenistasService))]
public class TenistasServiceTest
{
    [SetUp]
    public void Setup()
    {
        _mockCache = new Mock<ITenistasCache>();
        _mockLocalRepository = new Mock<ITenistasRepositoryLocal>();
        _mockRemoteRepository = new Mock<ITenistasRepositoryRemote>();
        _mockCsvStorage = new Mock<ITenistasStorageCsv>();
        _mockJsonStorage = new Mock<ITenistasStorageJson>();
        _mockNotificationsService = new Mock<ITenistasNotifications>();

        _tenistasService = new TenistasService(
            _mockLocalRepository.Object,
            _mockRemoteRepository.Object,
            _mockCache.Object,
            _mockCsvStorage.Object,
            _mockJsonStorage.Object,
            _mockNotificationsService.Object,
            10000 // cualquier tiempo de refresco razonable
        );
    }

    private Mock<ITenistasCache> _mockCache;
    private Mock<ITenistasRepositoryLocal> _mockLocalRepository;
    private Mock<ITenistasRepositoryRemote> _mockRemoteRepository;
    private Mock<ITenistasStorageCsv> _mockCsvStorage;
    private Mock<ITenistasStorageJson> _mockJsonStorage;
    private Mock<ITenistasNotifications> _mockNotificationsService;
    private TenistasService _tenistasService;

    private readonly Tenista testTenista =
        new("Test1", "Suiza", 185, 85, 2000, Mano.Diestro, new DateTime(1981, 8, 8), id: 1);

    [Test]
    [DisplayName("Debe obtener todos los tenistas desde el repositorio local cuando fromRemote es false")]
    public async Task GetAllAsync_DesdeLocal()
    {
        // Arrange
        _mockLocalRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(Result.Success<List<Tenista>, TenistaError>([testTenista]));

        // Act
        var result = await _tenistasService.GetAllAsync(false);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True, "El resultado debe ser exitoso");
            Assert.That(result.Value[0], Is.EqualTo(testTenista),
                "Los tenistas devueltos deben ser los mismos que los del repositorio local");
        });

        _mockLocalRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    [Test]
    [DisplayName("Debe obtener un tenista por id desde el cache y el repositorio local o remoto")]
    public async Task GetByIdAsync_DesdeCacheYRepositorios()
    {
        // Arrange
        _mockCache.Setup(cache => cache.Get(It.IsAny<long>())).Returns(testTenista);
        _mockLocalRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<long>()))
            .ReturnsAsync(Result.Success<Tenista, TenistaError>(testTenista));
        _mockRemoteRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<long>()))
            .ReturnsAsync(Result.Success<Tenista, TenistaError>(testTenista));

        // Act
        var result = await _tenistasService.GetByIdAsync(1);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True, "El resultado debe ser exitoso");
            Assert.That(result.Value, Is.EqualTo(testTenista),
                "El tenista devuelto debe ser el mismo que el del repositorio local o remoto");
        });

        _mockCache.Verify(cache => cache.Get(It.IsAny<long>()), Times.Once);
        _mockLocalRepository.Verify(repo => repo.GetByIdAsync(1), Times.Never);
        _mockRemoteRepository.Verify(repo => repo.GetByIdAsync(1), Times.Never);
    }

    [Test]
    [DisplayName("Debe obtener un tenista de la cache si está presente")]
    public async Task GetByIdAsync_DesdeCache()
    {
        // Arrange
        _mockCache.Setup(cache => cache.Get(It.IsAny<long>())).Returns(testTenista);

        // Act
        var result = await _tenistasService.GetByIdAsync(1);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True, "El resultado debe ser exitoso");
            Assert.That(result.Value, Is.EqualTo(testTenista),
                "El tenista devuelto debe ser el mismo que el de la cache");
        });

        _mockCache.Verify(cache => cache.Get(It.IsAny<long>()), Times.Once);
        _mockLocalRepository.Verify(repo => repo.GetByIdAsync(1), Times.Never);
        _mockRemoteRepository.Verify(repo => repo.GetByIdAsync(1), Times.Never);
    }

    [Test]
    [DisplayName("Debe obtener un tenista del repositorio local si no está en la cache")]
    public async Task GetByIdAsync_DesdeLocal()
    {
        // Arrange
        _mockCache.Setup(cache => cache.Get(It.IsAny<long>())).Returns((Tenista?)null);
        _mockLocalRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<long>()))
            .ReturnsAsync(Result.Success<Tenista, TenistaError>(testTenista));

        // Act
        var result = await _tenistasService.GetByIdAsync(1);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True, "El resultado debe ser exitoso");
            Assert.That(result.Value, Is.EqualTo(testTenista),
                "El tenista devuelto debe ser el mismo que el del repositorio local");
        });

        _mockCache.Verify(cache => cache.Get(It.IsAny<long>()), Times.Once);
        _mockLocalRepository.Verify(repo => repo.GetByIdAsync(1), Times.Once);
        _mockRemoteRepository.Verify(repo => repo.GetByIdAsync(1), Times.Never);
    }

    [Test]
    [DisplayName("Debe obtener un tenista del repositorio remoto si no está en la cache ni en el local")]
    public async Task GetByIdAsync_DesdeRemote()
    {
        // Arrange
        _mockCache.Setup(cache => cache.Get(It.IsAny<long>())).Returns((Tenista?)null);
        _mockLocalRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<long>()))
            .ReturnsAsync(Result.Failure<Tenista, TenistaError>(new TenistaError.DatabaseError("")));
        _mockRemoteRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<long>()))
            .ReturnsAsync(Result.Success<Tenista, TenistaError>(testTenista));

        // Act
        var result = await _tenistasService.GetByIdAsync(1);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True, "El resultado debe ser exitoso");
            Assert.That(result.Value, Is.EqualTo(testTenista),
                "El tenista devuelto debe ser el mismo que el del repositorio remoto");
        });

        _mockCache.Verify(cache => cache.Get(It.IsAny<long>()), Times.Once);
        _mockLocalRepository.Verify(repo => repo.GetByIdAsync(1), Times.Once);
        _mockRemoteRepository.Verify(repo => repo.GetByIdAsync(1), Times.Once);
    }

    [Test]
    [DisplayName("Debe devolver un error si no se encuentra el tenista en la cache, el repositorio local o el remoto")]
    public async Task GetByIdAsync_Error()
    {
        // Arrange
        _mockCache.Setup(cache => cache.Get(It.IsAny<long>())).Returns((Tenista?)null);
        _mockLocalRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<long>()))
            .ReturnsAsync(Result.Failure<Tenista, TenistaError>(new TenistaError.DatabaseError("")));
        _mockRemoteRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<long>()))
            .ReturnsAsync(Result.Failure<Tenista, TenistaError>(new TenistaError.RemoteError("404")));

        // Act
        var result = await _tenistasService.GetByIdAsync(1);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True, "El resultado debe ser un error");
            Assert.That(result.Error, Is.InstanceOf<TenistaError.RemoteError>(),
                "El error debe ser de tipo RemoteError");
            Assert.That(result.Error.ToString(), Is.EqualTo("ERROR: 404"),
                "El mensaje del error debe ser el esperado");
        });

        _mockCache.Verify(cache => cache.Get(It.IsAny<long>()), Times.Once);
        _mockLocalRepository.Verify(repo => repo.GetByIdAsync(1), Times.Once);
        _mockRemoteRepository.Verify(repo => repo.GetByIdAsync(1), Times.Once);
    }

    [Test]
    [DisplayName("Debe guardar un tenista con éxito")]
    public async Task SaveAsync_Exito()
    {
        // Arrange
        _mockRemoteRepository.Setup(repo => repo.SaveAsync(It.IsAny<Tenista>()))
            .ReturnsAsync(Result.Success<Tenista, TenistaError>(testTenista));
        _mockLocalRepository.Setup(repo => repo.SaveAsync(It.IsAny<Tenista>()))
            .ReturnsAsync(Result.Success<Tenista, TenistaError>(testTenista));
        _mockCache.Setup(cache => cache.Put(It.IsAny<long>(), It.IsAny<Tenista>()));
        _mockNotificationsService.Setup(service => service.Send(It.IsAny<Notification<TenistaDto>>()));

        // Act
        var result = await _tenistasService.SaveAsync(testTenista);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True, "El resultado debe ser exitoso");
            Assert.That(result.Value, Is.EqualTo(testTenista),
                "El tenista devuelto debe ser el mismo que el guardado");
        });

        _mockRemoteRepository.Verify(repo => repo.SaveAsync(It.IsAny<Tenista>()), Times.Once);
        _mockLocalRepository.Verify(repo => repo.SaveAsync(It.IsAny<Tenista>()), Times.Once);
        _mockCache.Verify(cache => cache.Put(It.IsAny<long>(), It.IsAny<Tenista>()), Times.Once);
        _mockNotificationsService.Verify(service => service.Send(It.IsAny<Notification<TenistaDto>>()), Times.Once);
    }

    [Test]
    [DisplayName("Debe fallar al guardar un tenista por validación")]
    public async Task SaveAsync_FalloValidacion()
    {
        // Arrange
        var invalidTenista = new Tenista("Test", "", 0, 0, 0, Mano.Diestro, DateTime.MinValue, id: 2);
        _mockLocalRepository.Setup(repo => repo.SaveAsync(It.IsAny<Tenista>()))
            .ReturnsAsync(
                Result.Failure<Tenista, TenistaError>(
                    new TenistaError.ValidationError("ERROR: La altura debe ser mayor a 0")));

        // Act
        var result = await _tenistasService.SaveAsync(invalidTenista);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False, "El resultado debe ser fallido");
            Assert.That(result.Error, Is.InstanceOf<TenistaError.ValidationError>(),
                "Debe fallar por un error de validación");
            Assert.That(result.Error.ToString(), Is.EqualTo("ERROR: La altura debe ser mayor a 0"),
                "El mensaje del error debe ser el esperado");
        });

        _mockRemoteRepository.Verify(repo => repo.SaveAsync(It.IsAny<Tenista>()), Times.Never);
    }

    [Test]
    [DisplayName("Debe actualizar un tenista con éxito")]
    public async Task UpdateAsync_Exito()
    {
        // Arrange
        _mockLocalRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<long>()))
            .ReturnsAsync(Result.Success<Tenista, TenistaError>(testTenista));
        _mockRemoteRepository.Setup(repo => repo.UpdateAsync(It.IsAny<long>(), It.IsAny<Tenista>()))
            .ReturnsAsync(Result.Success<Tenista, TenistaError>(testTenista));
        _mockLocalRepository.Setup(repo => repo.UpdateAsync(It.IsAny<long>(), It.IsAny<Tenista>()))
            .ReturnsAsync(Result.Success<Tenista, TenistaError>(testTenista));
        _mockNotificationsService.Setup(service => service.Send(It.IsAny<Notification<TenistaDto>>()));
        _mockCache.Setup(cache => cache.Put(It.IsAny<long>(), It.IsAny<Tenista>()));

        // Act
        var result = await _tenistasService.UpdateAsync(1, testTenista);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True, "El resultado debe ser exitoso");
            Assert.That(result.Value, Is.EqualTo(testTenista),
                "El tenista devuelto debe ser el mismo que el actualizado");
        });
        _mockLocalRepository.Verify(repo => repo.GetByIdAsync(1), Times.Once);
        _mockRemoteRepository.Verify(repo => repo.UpdateAsync(It.IsAny<long>(), It.IsAny<Tenista>()), Times.Once);
        _mockLocalRepository.Verify(repo => repo.UpdateAsync(It.IsAny<long>(), It.IsAny<Tenista>()), Times.Once);
        _mockNotificationsService.Verify(service => service.Send(It.IsAny<Notification<TenistaDto>>()), Times.Once);
        _mockCache.Verify(cache => cache.Put(It.IsAny<long>(), It.IsAny<Tenista>()), Times.Once);
    }

    [Test]
    [DisplayName("Debe fallar al actualizar un tenista no existente")]
    public async Task UpdateAsync_TenistaNoExistente()
    {
        // Arrange
        _mockRemoteRepository.Setup(repo => repo.UpdateAsync(It.IsAny<long>(), testTenista))
            .ReturnsAsync(Result.Failure<Tenista, TenistaError>(new TenistaError.RemoteError("404")));
        // Act
        var result = await _tenistasService.UpdateAsync(1, testTenista);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False, "El resultado debe ser fallido");
            Assert.That(result.Error, Is.InstanceOf<TenistaError.RemoteError>(),
                "Debe fallar por un error remoto");
            Assert.That(result.Error.ToString(), Is.EqualTo("ERROR: 404"),
                "El mensaje del error debe ser el esperado");
        });

        _mockRemoteRepository.Verify(repo => repo.UpdateAsync(It.IsAny<long>(), It.IsAny<Tenista>()), Times.Once);
        _mockLocalRepository.Verify(repo => repo.UpdateAsync(It.IsAny<long>(), It.IsAny<Tenista>()), Times.Never);
    }

    [Test]
    [DisplayName("Debe fallar al actualizar un tenista por validación")]
    public async Task UpdateAsync_FalloValidacion()
    {
        // Arrange
        var invalidTenista = new Tenista("Test", "", 0, 0, 0, Mano.Diestro, DateTime.MinValue, id: 2);
        _mockLocalRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<long>()))
            .ReturnsAsync(Result.Success<Tenista, TenistaError>(testTenista));
        _mockRemoteRepository.Setup(repo => repo.UpdateAsync(It.IsAny<long>(), It.IsAny<Tenista>()))
            .ReturnsAsync(
                Result.Failure<Tenista, TenistaError>(
                    new TenistaError.ValidationError("ERROR: La altura debe ser mayor a 0")));

        // Act
        var result = await _tenistasService.UpdateAsync(1, invalidTenista);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False, "El resultado debe ser fallido");
            Assert.That(result.Error, Is.InstanceOf<TenistaError.ValidationError>(),
                "Debe fallar por un error de validación");
            Assert.That(result.Error.ToString(), Is.EqualTo("ERROR: La altura debe ser mayor a 0"),
                "El mensaje del error debe ser el esperado");
        });

        _mockRemoteRepository.Verify(repo => repo.UpdateAsync(It.IsAny<long>(), It.IsAny<Tenista>()), Times.Never);
        _mockLocalRepository.Verify(repo => repo.GetByIdAsync(1), Times.Never);
        _mockLocalRepository.Verify(repo => repo.UpdateAsync(It.IsAny<long>(), It.IsAny<Tenista>()), Times.Never);
        _mockNotificationsService.Verify(service => service.Send(It.IsAny<Notification<TenistaDto>>()), Times.Never);
        _mockCache.Verify(cache => cache.Put(It.IsAny<long>(), It.IsAny<Tenista>()), Times.Never);
        _mockCache.Verify(cache => cache.Remove(It.IsAny<long>()), Times.Never);
    }

    [Test]
    [DisplayName("Debe eliminar un tenista con éxito")]
    public async Task DeleteAsync_Exito()
    {
        // Arrange
        _mockLocalRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<long>()))
            .ReturnsAsync(Result.Success<Tenista, TenistaError>(testTenista));
        _mockRemoteRepository.Setup(repo => repo.DeleteAsync(It.IsAny<long>()))
            .ReturnsAsync(Result.Success<long, TenistaError>(1));
        _mockLocalRepository.Setup(repo => repo.DeleteAsync(It.IsAny<long>()))
            .ReturnsAsync(Result.Success<long, TenistaError>(1));

        // Act
        var result = await _tenistasService.DeleteAsync(1);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True, "El resultado debe ser exitoso");
            Assert.That(result.Value, Is.EqualTo(1),
                "El ID del tenista eliminado debe ser el mismo que el solicitado");
        });

        _mockRemoteRepository.Verify(repo => repo.DeleteAsync(It.IsAny<long>()), Times.Once);
        _mockLocalRepository.Verify(repo => repo.DeleteAsync(It.IsAny<long>()), Times.Once);
        _mockCache.Verify(cache => cache.Remove(It.IsAny<long>()), Times.Once);
        _mockNotificationsService.Verify(service => service.Send(It.IsAny<Notification<TenistaDto>>()), Times.Once);
    }

    [Test]
    [DisplayName("Debe fallar al eliminar un tenista no existente")]
    public async Task DeleteAsync_TenistaNoExistente()
    {
        // Arrange
        _mockRemoteRepository.Setup(repo => repo.DeleteAsync(It.IsAny<long>()))
            .ReturnsAsync(Result.Failure<long, TenistaError>(new TenistaError.RemoteError("404")));

        // Act
        var result = await _tenistasService.DeleteAsync(1);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False, "El resultado debe ser fallido");
            Assert.That(result.Error, Is.InstanceOf<TenistaError.RemoteError>(),
                "Debe fallar por un error remoto");
            Assert.That(result.Error.ToString(), Is.EqualTo("ERROR: 404"),
                "El mensaje del error debe ser el esperado");
        });

        _mockRemoteRepository.Verify(repo => repo.DeleteAsync(It.IsAny<long>()), Times.Once);
        _mockLocalRepository.Verify(repo => repo.DeleteAsync(It.IsAny<long>()), Times.Never);
        _mockCache.Verify(cache => cache.Remove(It.IsAny<long>()), Times.Never);
        _mockNotificationsService.Verify(service => service.Send(It.IsAny<Notification<TenistaDto>>()), Times.Never);
    }

    [Test]
    [DisplayName("Debe importar datos desde un archivo CSV")]
    public async Task ImportDataAsync_ImportarCsv()
    {
        // Arrange
        var fileInfo = new FileInfo("test.csv");
        _mockCsvStorage.Setup(storage => storage.ImportAsync(It.IsAny<FileInfo>()))
            .ReturnsAsync(Result.Success<List<Tenista>, TenistaError.StorageError>([testTenista]));
        _mockLocalRepository.Setup(repo => repo.SaveAllAsync(It.IsAny<List<Tenista>>()))
            .ReturnsAsync(Result.Success<int, TenistaError>(1));

        // Act
        var result = await _tenistasService.ImportDataAsync(fileInfo);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True, "El resultado debe ser exitoso");
            Assert.That(result.Value, Is.EqualTo(1), "Debe devolver el número de tenistas importados");
        });

        _mockCsvStorage.Verify(storage => storage.ImportAsync(fileInfo), Times.Once);
    }


    [Test]
    [DisplayName("Debe importar datos desde un archivo JSON")]
    public async Task ImportDataAsync_ImportarJson()
    {
        // Arrange
        var fileInfo = new FileInfo("test.json");
        _mockJsonStorage.Setup(storage => storage.ImportAsync(It.IsAny<FileInfo>()))
            .ReturnsAsync(Result.Success<List<Tenista>, TenistaError.StorageError>([testTenista]));
        _mockLocalRepository.Setup(repo => repo.SaveAllAsync(It.IsAny<List<Tenista>>()))
            .ReturnsAsync(Result.Success<int, TenistaError>(1));

        // Act
        var result = await _tenistasService.ImportDataAsync(fileInfo);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True, "El resultado debe ser exitoso");
            Assert.That(result.Value, Is.EqualTo(1), "Debe devolver el número de tenistas importados");
        });

        _mockJsonStorage.Verify(storage => storage.ImportAsync(fileInfo), Times.Once);
    }

    [Test]
    [DisplayName("Debe fallar al importar con un formato no soportado")]
    public async Task ImportDataAsync_FormatoNoSoportado()
    {
        // Arrange
        var fileInfo = new FileInfo("test.txt");

        // Act
        var result = await _tenistasService.ImportDataAsync(fileInfo);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True, "El resultado debe ser fallido");
            Assert.That(result.Error, Is.InstanceOf<TenistaError.StorageError>(),
                "El error debe ser de tipo StorageError");
            Assert.That(result.Error.ToString(), Contains.Substring("Formato de archivo no soportado"),
                "El mensaje de error debe indicar que el formato no está soportado");
        });
    }

    [Test]
    [DisplayName("Debe exportar datos a un archivo CSV")]
    public async Task ExportDataAsync_ExportarCsv()
    {
        // Arrange
        var fileInfo = new FileInfo("test.csv");
        _mockCsvStorage.Setup(storage => storage.ExportAsync(It.IsAny<FileInfo>(), It.IsAny<List<Tenista>>()))
            .ReturnsAsync(Result.Success<int, TenistaError.StorageError>(1));
        _mockLocalRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(Result.Success<List<Tenista>, TenistaError>([testTenista]));

        // Act
        var result = await _tenistasService.ExportDataAsync(fileInfo, false);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True, "El resultado debe ser exitoso");
            Assert.That(result.Value, Is.EqualTo(1), "Debe devolver el número de tenistas exportados");
        });

        _mockCsvStorage.Verify(storage => storage.ExportAsync(It.IsAny<FileInfo>(), It.IsAny<List<Tenista>>()),
            Times.Once);
    }

    [Test]
    [DisplayName("Debe exportar datos a un archivo JSON")]
    public async Task ExportDataAsync_ExportarJson()
    {
        // Arrange
        var fileInfo = new FileInfo("test.json");
        _mockJsonStorage.Setup(storage => storage.ExportAsync(It.IsAny<FileInfo>(), It.IsAny<List<Tenista>>()))
            .ReturnsAsync(Result.Success<int, TenistaError.StorageError>(1));
        _mockLocalRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(Result.Success<List<Tenista>, TenistaError>([testTenista]));

        // Act
        var result = await _tenistasService.ExportDataAsync(fileInfo, false);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True, "El resultado debe ser exitoso");
            Assert.That(result.Value, Is.EqualTo(1), "Debe devolver el número de tenistas exportados");
        });

        _mockJsonStorage.Verify(storage => storage.ExportAsync(It.IsAny<FileInfo>(), It.IsAny<List<Tenista>>()),
            Times.Once);
    }

    [Test]
    [DisplayName("Debe fallar al exportar con un formato no soportado")]
    public async Task ExportDataAsync_FormatoNoSoportado()
    {
        // Arrange
        var fileInfo = new FileInfo("test.txt");

        // Act
        var result = await _tenistasService.ExportDataAsync(fileInfo, false);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True, "El resultado debe ser fallido");
            Assert.That(result.Error, Is.InstanceOf<TenistaError.StorageError>(),
                "El error debe ser de tipo StorageError");
            Assert.That(result.Error.ToString(), Contains.Substring("Formato de archivo no soportado"),
                "El mensaje de error debe indicar que el formato no está soportado");
        });
    }

    [Test]
    [DisplayName("Debe habilitar el auto refresco")]
    public async Task EnableAutoRefresh_Habilitar()
    {
        // Arrange
        _mockLocalRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(Result.Success<List<Tenista>, TenistaError>(new List<Tenista> { testTenista }));
        _mockRemoteRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(Result.Success<List<Tenista>, TenistaError>(new List<Tenista> { testTenista }));
        _mockLocalRepository.Setup(repo => repo.RemoveAllAsync());
        _mockLocalRepository.Setup(repo => repo.SaveAllAsync(It.IsAny<List<Tenista>>()))
            .ReturnsAsync(Result.Success<int, TenistaError>(1));

        // Act
        _tenistasService.EnableAutoRefresh();
        await Task.Delay(1000); // Espera para que el auto refresco se ejecute

        // Assert
        _mockLocalRepository.Verify(repo => repo.GetAllAsync(), Times.Never);
        _mockRemoteRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        _mockLocalRepository.Verify(repo => repo.RemoveAllAsync(), Times.Once);
        _mockLocalRepository.Verify(repo => repo.SaveAllAsync(It.IsAny<List<Tenista>>()), Times.Once);
    }
}