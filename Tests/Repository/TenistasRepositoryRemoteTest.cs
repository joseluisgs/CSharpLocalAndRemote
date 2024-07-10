using System.ComponentModel;
using CSharpLocalAndRemote.Dto;
using CSharpLocalAndRemote.Error;
using CSharpLocalAndRemote.model;
using CSharpLocalAndRemote.Repository;
using CSharpLocalAndRemote.Rest;
using Moq;

namespace Tests.Repository;

[TestFixture]
[TestOf(typeof(TenistasRepositoryRemote))]
public class TenistasRepositoryRemoteTest
{
    [SetUp]
    public void Setup()
    {
        _mockApi = new Mock<ITenistasApiRest>(); // Inicializamos el mock
        _repository =
            new TenistasRepositoryRemote(_mockApi
                .Object); // Inicializamos el repositorio con el objeto que devuelve el mock
    }

    private Mock<ITenistasApiRest> _mockApi; // Mock de la interfaz ITenistasApiRest
    private TenistasRepositoryRemote _repository; // Repositorio a testear

    private readonly List<Tenista> tenistas =
    [
        new Tenista("Test1", "USA", 185, 85, 2000, Mano.Diestro, new DateTime(1981, 8, 8), id: 100)
    ];

    private readonly List<TenistaDto> tenistaDtos = new()
    {
        new TenistaDto(100, "Test1", "USA", 185, 75, 2000, "DIESTRO", "1981-08-08", "2023-01-01T00:00:00Z",
            "2023-01-01T00:00:00Z")
    };

    [Test]
    [DisplayName("Obtener todos los tenistas remotos correctamente")]
    public async Task ObtenerTodosLosTenistasRemotos_Correctamente()
    {
        // Arrange
        _mockApi.Setup(api => api.GetAllAsync())
            .ReturnsAsync(tenistaDtos); // Configuramos el mock para que devuelva tenistaDtos

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True, "El resultado debe indicar éxito.");
            Assert.That(result.Value, Has.Count.EqualTo(1), "Debe haber un tenista en la lista.");
            Assert.That(result.Value[0].Id, Is.EqualTo(100), "El id del tenista debe ser 100.");
            Assert.That(result.Value[0].Nombre, Is.EqualTo("Test1"), "El nombre del tenista debe ser 'Test1'.");
        });

        _mockApi.Verify(api => api.GetAllAsync(), Times.Once); // Verificamos que se llamó al método GetAllAsync
    }

    [Test]
    [DisplayName("Obtener todos los tenistas remotos con error")]
    public async Task ObtenerTodosLosTenistasRemotos_ConError()
    {
        // Arrange
        _mockApi.Setup(api => api.GetAllAsync())
            .ThrowsAsync(new Exception("Error de la API.")); // Configuramos el mock para que devuelva una excepción

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True, "El resultado debe indicar fallido.");
            Assert.That(result.Error, Is.InstanceOf<TenistaError.RemoteError>(), "El error debe ser de tipo ApiError.");
            Assert.That(result.Error.ToString().Contains("ERROR: No se pueden obtener los tenistas remotos"),
                "El mensaje de error debe contener 'Error de la API'");
        });

        _mockApi.Verify(api => api.GetAllAsync(), Times.Once); // Verificamos que se llamó al método GetAllAsync
    }

    [Test]
    [DisplayName("Obtener tenista remoto por id correctamente")]
    public async Task ObtenerTenistaRemotoPorId_Correctamente()
    {
        // Arrange
        _mockApi.Setup(api => api.GetByIdAsync(1L))
            .ReturnsAsync(tenistaDtos[0]); // Configuramos el mock para que devuelva tenistaDtos

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True, "El resultado debe indicar éxito.");
            Assert.That(result.Value.Id, Is.EqualTo(100), "El id del tenista debe ser 100.");
            Assert.That(result.Value.Nombre, Is.EqualTo("Test1"), "El nombre del tenista debe ser 'Test1'.");
        });

        _mockApi.Verify(api => api.GetByIdAsync(1L),
            Times.Once); // Verificamos que se llamó al método GetByIdAsync
    }

    [Test]
    [DisplayName("Obtener tenista remoto por id con error")]
    public async Task ObtenerTenistaRemotoPorId_ConError()
    {
        // Arrange
        _mockApi.Setup(api => api.GetByIdAsync(It.IsAny<int>()))
            .ThrowsAsync(new Exception("Error de la API.")); // Configuramos el mock para que devuelva una excepción

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True, "El resultado debe indicar fallido.");
            Assert.That(result.Error, Is.InstanceOf<TenistaError.RemoteError>(), "El error debe ser de tipo ApiError.");
            Assert.That(result.Error.ToString().Contains("ERROR: No se ha obteniendo el tenista remoto con id 1"),
                Is.True, "ERROR: No se ha obteniendo el tenista remoto con id 1.");
        });

        _mockApi.Verify(api => api.GetByIdAsync(1), Times.Once); // Verificamos que se llamó al método GetByIdAsync
    }

    [Test]
    [DisplayName("Guardar tenista remoto correctamente")]
    public async Task GuardarTenistaRemoto_Correctamente()
    {
        // Arrange
        _mockApi.Setup(api => api.SaveAsync(It.IsAny<TenistaDto>())).ReturnsAsync(tenistaDtos.First());

        // Act
        var result = await _repository.SaveAsync(tenistas.First());

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True, "El resultado debe indicar éxito.");
            Assert.That(result.Value.Id, Is.EqualTo(100), "El id del tenista debe ser 100.");
            Assert.That(result.Value.Nombre, Is.EqualTo("Test1"), "El nombre del tenista debe ser 'Test1'.");
        });

        _mockApi.Verify(api => api.SaveAsync(It.IsAny<TenistaDto>()),
            Times.Once); // Verificamos que se llamó al método SaveAsync
    }


    [Test]
    [DisplayName("Guardar tenista remoto con error")]
    public async Task GuardarTenistaRemoto_ConError()
    {
        // Arrange
        _mockApi.Setup(api => api.SaveAsync(It.IsAny<TenistaDto>())).ThrowsAsync(new Exception("Error de la API."));

        // Act
        var result = await _repository.SaveAsync(tenistas.First());

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True, "El resultado debe indicar fallido.");
            Assert.That(result.Error, Is.InstanceOf<TenistaError.RemoteError>(),
                "El error debe ser de tipo RemoteError.");
            Assert.That(result.Error.ToString(), Does.Contain("No se ha guardado el tenista remoto"),
                "El mensaje de error debe contener 'No se ha guardado el tenista remoto'");
        });

        _mockApi.Verify(api => api.SaveAsync(It.IsAny<TenistaDto>()),
            Times.Once); // Verificamos que se llamó al método SaveAsync
    }

    [Test]
    [DisplayName("Actualizar tenista remoto correctamente")]
    public async Task ActualizarTenistaRemoto_Correctamente()
    {
        // Arrange
        _mockApi.Setup(api => api.UpdateAsync(1, It.IsAny<TenistaDto>())).ReturnsAsync(tenistaDtos.First());

        // Act
        var result = await _repository.UpdateAsync(1, tenistas.First());

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True, "El resultado debe indicar éxito.");
            Assert.That(result.Value.Id, Is.EqualTo(100), "El id del tenista debe ser 100.");
            Assert.That(result.Value.Nombre, Is.EqualTo("Test1"), "El nombre del tenista debe ser 'Test1'.");
        });

        _mockApi.Verify(api => api.UpdateAsync(1, It.IsAny<TenistaDto>()),
            Times.Once); // Verificamos que se llamó al método UpdateAsync
    }

    [Test]
    [DisplayName("Actualizar tenista remoto con error")]
    public async Task ActualizarTenistaRemoto_ConError()
    {
        // Arrange
        _mockApi.Setup(api => api.UpdateAsync(1, It.IsAny<TenistaDto>()))
            .ThrowsAsync(new Exception("Error de la API."));

        // Act
        var result = await _repository.UpdateAsync(1, tenistas.First());

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True, "El resultado debe indicar fallido.");
            Assert.That(result.Error, Is.InstanceOf<TenistaError.RemoteError>(),
                "El error debe ser de tipo RemoteError.");
            Assert.That(result.Error.ToString(), Does.Contain("No se ha actualizado el tenista remoto"),
                "El mensaje de error debe contener 'No se ha actualizado el tenista remoto'");
        });

        _mockApi.Verify(api => api.UpdateAsync(1, It.IsAny<TenistaDto>()),
            Times.Once); // Verificamos que se llamó al método UpdateAsync
    }

    [Test]
    [DisplayName("Actualizar tenista debe devolver un error cuando el tenista no existe")]
    public async Task ActualizarTenistaRemoto_NoExiste()
    {
        // Arrange
        _mockApi.Setup(api => api.UpdateAsync(100, It.IsAny<TenistaDto>()))
            .ThrowsAsync(new Exception("Tenista no encontrado."));

        // Act
        var result = await _repository.UpdateAsync(100, tenistas.First());

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True, "El resultado debe indicar fallido.");
            Assert.That(result.Error, Is.InstanceOf<TenistaError.RemoteError>(),
                "El error debe ser de tipo RemoteError.");
            Assert.That(result.Error.ToString(), Does.Contain("No se ha actualizado el tenista remoto"),
                "El mensaje de error debe contener 'No se ha actualizado el tenista remoto'");
        });

        _mockApi.Verify(api => api.UpdateAsync(100, It.IsAny<TenistaDto>()),
            Times.Once); // Verificamos que se llamó al método UpdateAsync
    }

    [Test]
    [DisplayName("Borrar tenista remoto correctamente")]
    public async Task BorrarTenistaRemoto_Correctamente()
    {
        // Arrange
        _mockApi.Setup(api => api.DeleteAsync(1));

        // Act
        var result = await _repository.DeleteAsync(1);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True, "El resultado debe indicar éxito.");
            Assert.That(result.Value, Is.EqualTo(1), "El id del tenista eliminado debe ser 1.");
        });

        _mockApi.Verify(api => api.DeleteAsync(1), Times.Once); // Verificamos que se llamó al método DeleteAsync
    }

    [Test]
    [DisplayName("Borrar tenista remoto con error")]
    public async Task BorrarTenistaRemoto_ConError()
    {
        // Arrange
        _mockApi.Setup(api => api.DeleteAsync(1))
            .ThrowsAsync(new Exception("Error de la API."));

        // Act
        var result = await _repository.DeleteAsync(1);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True, "El resultado debe indicar fallido.");
            Assert.That(result.Error, Is.InstanceOf<TenistaError.RemoteError>(),
                "El error debe ser de tipo RemoteError.");
            Assert.That(result.Error.ToString(), Does.Contain("No se ha eliminado el tenista remoto"),
                "El mensaje de error debe contener 'No se ha eliminado el tenista remoto'");
        });

        _mockApi.Verify(api => api.DeleteAsync(1), Times.Once); // Verificamos que se llamó al método DeleteAsync
    }

    [Test]
    [DisplayName("Borrar tenista debe devolver un error cuando el tenista no existe")]
    public async Task BorrarTenistaRemoto_NoExiste()
    {
        // Arrange
        _mockApi.Setup(api => api.DeleteAsync(100))
            .ThrowsAsync(new Exception("Tenista no encontrado."));

        // Act
        var result = await _repository.DeleteAsync(100);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True, "El resultado debe indicar fallido.");
            Assert.That(result.Error, Is.InstanceOf<TenistaError.RemoteError>(),
                "El error debe ser de tipo RemoteError.");
            Assert.That(result.Error.ToString(), Does.Contain("No se ha eliminado el tenista remoto"),
                "El mensaje de error debe contener 'No se ha eliminado el tenista remoto'");
        });

        _mockApi.Verify(api => api.DeleteAsync(100), Times.Once); // Verificamos que se llamó al método DeleteAsync
    }
}