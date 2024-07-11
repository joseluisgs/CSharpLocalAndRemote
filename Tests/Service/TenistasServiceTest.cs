using System.ComponentModel;
using CSharpFunctionalExtensions;
using CSharpLocalAndRemote.Cache;
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
        _mockCsvStorage = new Mock<TenistasStorageCsv>();
        _mockJsonStorage = new Mock<TenistasStorageJson>();
        _mockNotificationsService = new Mock<TenistasNotifications>();

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
    private Mock<TenistasStorageCsv> _mockCsvStorage;
    private Mock<TenistasStorageJson> _mockJsonStorage;
    private Mock<TenistasNotifications> _mockNotificationsService;
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
}