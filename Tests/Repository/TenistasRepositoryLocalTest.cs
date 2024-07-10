using System.ComponentModel;
using CSharpLocalAndRemote.Database;
using CSharpLocalAndRemote.Error;
using CSharpLocalAndRemote.Mapper;
using CSharpLocalAndRemote.model;
using CSharpLocalAndRemote.Repository;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Tests;

public class TenistasRepositoryLocalTest
{
    private SqliteConnection _connection;
    private TenistasDbContext _context;
    private TenistasRepositoryLocal _repository;

    [SetUp]
    public void Setup()
    {
        /*
         * Necesitamos la conexión a la base de datos en memoria para realizar las pruebas.
         * Ya que no es una base de datos en ficheros donde el propio driver se encarga de abrir y cerrar la conexión,
         * necesitamos hacerlo nosotros en el SetUp y en el TearDown.
         */
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open(); // Abrir la conexión

        // Configurar opciones de DbContext para usar SQLite en memoria
        var options = new DbContextOptionsBuilder<TenistasDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new TenistasDbContext(options); // Crear el contexto de la base de datos
        _context.Database.EnsureCreated(); // Crear la base de datos en memoria si no existe

        _repository = new TenistasRepositoryLocal(_context); // Crear el repositorio

        // Datos que se insertarán en la base de datos en memoria
        var tenistas = new List<Tenista>
        {
            new("Test1", "Suiza", 185, 85, 2000, Mano.Diestro, new DateTime(1981, 8, 8), id: 1),
            new("Test2", "Serbia", 188, 77, 1500, Mano.Diestro, new DateTime(1987, 5, 22), id: 2),
            new("Test3", "Inglaterra", 188, 76, 1500, Mano.Diestro, new DateTime(1987, 12, 12), id: 3)
        }.Select(tenista => tenista.ToTenistaEntity()).ToList();

        // Insertar los tenistas en la base de datos en memoria
        _context.Tenistas.AddRange(tenistas);
        _context.SaveChanges();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted(); // Eliminar la base de datos en memoria
        _context.Dispose(); // Liberar recursos del contexto
        _connection.Close(); // Cerrar la conexión SQLite en memoria
    }

    [Test]
    [DisplayName("GetAll debe devolver todos los tenistas")]
    public async Task GetAllAsync_DevuelveTodosLosTenistas()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True, "El resultado debe ser exitoso");
            Assert.That(result.Value.Count, Is.EqualTo(3), "Deben haber 3 tenistas en la lista");
            Assert.That(result.Value[0].Id, Is.EqualTo(1), "El id del primer tenista debe ser 1");
            Assert.That(result.Value[1].Id, Is.EqualTo(2), "El id del segundo tenista debe ser 2");
            Assert.That(result.Value[2].Id, Is.EqualTo(3), "El id del tercer tenista debe ser 3");
        });
    }

    [Test]
    [DisplayName("GetById debe devolver el tenista con el id correcto")]
    public async Task GetByIdAsync_DevuelveElTenistaConElIdCorrecto()
    {
        // Act
        var result = await _repository.GetByIdAsync(2);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True, "El resultado debe ser exitoso");
            Assert.That(result.Value.Id, Is.EqualTo(2), "El id del tenista debe ser 2");
            Assert.That(result.Value.Nombre, Is.EqualTo("Test2"), "El nombre del tenista debe ser 'Test2'");
        });
    }

    [Test]
    [DisplayName("GetById debe devolver un error cuando el tenista no existe")]
    public async Task GetByIdAsync_DevuelveErrorCuandoElTenistaNoExiste()
    {
        // Act
        var result = await _repository.GetByIdAsync(4);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True, "El resultado debe ser fallido");
            Assert.That(result.Error, Is.InstanceOf<TenistaError.DatabaseError>(),
                "El error debe ser de tipo DatabaseError");
            Assert.That(result.Error.ToString(),
                Is.EqualTo("ERROR: El tenista con id 4 no se encontró en la base de datos."),
                "Los mensajes de error deben ser iguales");
        });
    }

    [Test]
    [DisplayName("CreateAsync debe devolver el tenista creado")]
    public async Task CreateAsync_DevuelveElTenistaCreado()
    {
        // Arrange
        var tenista = new Tenista("Test4", "España", 190, 80, 1500, Mano.Diestro, new DateTime(1987, 12, 12), id: 4);

        // Act
        var result = await _repository.SaveAsync(tenista);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True, "El resultado debe ser exitoso");
            Assert.That(result.Value.Id, Is.EqualTo(4), "El id del tenista debe ser 4");
        });
    }

    [Test]
    [DisplayName("UpdateAsync debe devolver el tenista actualizado")]
    public async Task UpdateAsync_DevuelveElTenistaActualizado()
    {
        // Arrange
        var tenistaToUpdate =
            new Tenista("TestUpdate", "España", 191, 78, 1500, Mano.Diestro, new DateTime(1987, 5, 22), id: 2);

        // Act
        var result = await _repository.UpdateAsync(2, tenistaToUpdate);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True, "El resultado debe ser exitoso");
            Assert.That(result.Value.Id, Is.EqualTo(2), "El id del tenista debe ser 2");
            Assert.That(result.Value.Nombre, Is.EqualTo("TestUpdate"), "El nombre del tenista debe ser 'TestUpdate'");
            Assert.That(result.Value.Altura, Is.EqualTo(191), "La altura del tenista debe ser 191");
        });
    }

    [Test]
    [DisplayName("UpdateAsync debe devolver un error cuando el tenista no existe")]
    public async Task UpdateAsync_DevuelveErrorCuandoElTenistaNoExiste()
    {
        // Arrange
        var tenistaToUpdate =
            new Tenista("TestUpdate", "España", 191, 78, 1500, Mano.Diestro, new DateTime(1987, 5, 22), id: 4);

        // Act
        var result = await _repository.UpdateAsync(5, tenistaToUpdate);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True, "El resultado debe ser fallido");
            Assert.That(result.Error, Is.InstanceOf<TenistaError.DatabaseError>(),
                "El error debe ser de tipo DatabaseError");
            Assert.That(result.Error.ToString(),
                Is.EqualTo("ERROR: El tenista con id 5 no se encontró en la base de datos."),
                "Los mensajes de error deben ser iguales");
        });
    }

    [Test]
    [DisplayName("DeleteAsync debe devolver el id del tenista eliminado")]
    public async Task DeleteAsync_DevuelveElIdDelTenistaEliminado()
    {
        // Act
        var result = await _repository.DeleteAsync(2);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True, "El resultado debe ser exitoso");
            Assert.That(result.Value, Is.EqualTo(2), "El id del tenista eliminado debe ser 2");
        });
    }

    [Test]
    [DisplayName("DeleteAsync debe devolver un error cuando el tenista no existe")]
    public async Task DeleteAsync_DevuelveErrorCuandoElTenistaNoExiste()
    {
        // Act
        var result = await _repository.DeleteAsync(5);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True, "El resultado debe ser fallido");
            Assert.That(result.Error, Is.InstanceOf<TenistaError.DatabaseError>(),
                "El error debe ser de tipo DatabaseError");
            Assert.That(result.Error.ToString(),
                Is.EqualTo("ERROR: El tenista con id 5 no se encontró en la base de datos."),
                "Los mensajes de error deben ser iguales");
        });
    }
}