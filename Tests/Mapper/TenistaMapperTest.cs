using System.ComponentModel;
using CSharpLocalAndRemote.Database;
using CSharpLocalAndRemote.Dto;
using CSharpLocalAndRemote.Mapper;
using CSharpLocalAndRemote.model;

namespace Tests.Mapper;

[TestFixture]
[TestOf(typeof(TenistaMapper))]
public class TenistaMapperTest
{
    private readonly Tenista tenista = new(
        "Rafael Nadal",
        "España",
        185,
        85,
        10250,
        Mano.Diestro,
        new DateTime(1986, 6, 3),
        id: 1,
        isDeleted: false
    );

    private readonly TenistaDto tenistaDto = new(
        1,
        "Rafael Nadal",
        "España",
        185,
        85,
        10250,
        "Diestro",
        "1986-06-03",
        IsDeleted: false
    );

    private readonly TenistaEntity tenistaEntity = new()
    {
        Id = 1,
        Nombre = "Rafael Nadal",
        Pais = "España",
        Altura = 185,
        Peso = 85,
        Puntos = 10250,
        Mano = "Diestro",
        FechaNacimiento = "1986-06-03",
        IsDeleted = false,
        CreatedAt = DateTime.Now.ToString("o"),
        UpdatedAt = DateTime.Now.ToString("o")
    };

    [Test]
    [DisplayName("Convertir un TenistaDto en un Tenista")]
    public void ToTenista()
    {
        var testTenista = tenistaDto.ToTenista();
        Assert.Multiple(() =>
        {
            Assert.That(testTenista.Id, Is.EqualTo(tenista.Id), "Id deben ser iguales");
            Assert.That(testTenista.Nombre, Is.EqualTo(tenista.Nombre), "Nombre deben ser iguales");
            Assert.That(testTenista.Pais, Is.EqualTo(tenista.Pais), "Pais deben ser iguales");
            Assert.That(testTenista.Altura, Is.EqualTo(tenista.Altura), "Altura deben ser iguales");
            Assert.That(testTenista.Peso, Is.EqualTo(tenista.Peso), "Peso deben ser iguales");
            Assert.That(testTenista.Puntos, Is.EqualTo(tenista.Puntos), "Puntos deben ser iguales");
            Assert.That(testTenista.Mano, Is.EqualTo(tenista.Mano), "Mano deben ser iguales");
            Assert.That(testTenista.FechaNacimiento, Is.EqualTo(tenista.FechaNacimiento),
                "FechaNacimiento deben ser iguales");
            Assert.That(testTenista.IsDeleted, Is.EqualTo(tenista.IsDeleted), "IsDeleted deben ser iguales");
        });
    }

    [Test]
    [DisplayName("Convertir un Tenista en un TenistaDto")]
    public void ToTenistaDto()
    {
        var testDto = tenista.ToTenistaDto();
        Assert.Multiple(() =>
        {
            Assert.That(testDto.Id, Is.EqualTo(tenistaDto.Id), "Id deben ser iguales");
            Assert.That(testDto.Nombre, Is.EqualTo(tenistaDto.Nombre), "Nombre deben ser iguales");
            Assert.That(testDto.Pais, Is.EqualTo(tenistaDto.Pais), "Pais deben ser iguales");
            Assert.That(testDto.Altura, Is.EqualTo(tenistaDto.Altura), "Altura deben ser iguales");
            Assert.That(testDto.Peso, Is.EqualTo(tenistaDto.Peso), "Peso deben ser iguales");
            Assert.That(testDto.Puntos, Is.EqualTo(tenistaDto.Puntos), "Puntos deben ser iguales");
            Assert.That(testDto.Mano, Is.EqualTo(tenistaDto.Mano.ToUpper()), "Mano deben ser iguales");
            Assert.That(testDto.FechaNacimiento, Is.EqualTo(tenistaDto.FechaNacimiento),
                "FechaNacimiento deben ser iguales");
            Assert.That(testDto.IsDeleted, Is.EqualTo(tenistaDto.IsDeleted), "IsDeleted deben ser iguales");
        });
    }

    [Test]
    [DisplayName("Convertir un Tenista en un TenistaEntity")]
    public void ToTenistaEntity()
    {
        var testEntity = tenista.ToTenistaEntity();
        Assert.Multiple(() =>
        {
            Assert.That(testEntity.Id, Is.EqualTo(tenistaEntity.Id), "Id deben ser iguales");
            Assert.That(testEntity.Nombre, Is.EqualTo(tenistaEntity.Nombre), "Nombre deben ser iguales");
            Assert.That(testEntity.Pais, Is.EqualTo(tenistaEntity.Pais), "Pais deben ser iguales");
            Assert.That(testEntity.Altura, Is.EqualTo(tenistaEntity.Altura), "Altura deben ser iguales");
            Assert.That(testEntity.Peso, Is.EqualTo(tenistaEntity.Peso), "Peso deben ser iguales");
            Assert.That(testEntity.Puntos, Is.EqualTo(tenistaEntity.Puntos), "Puntos deben ser iguales");
        });
    }

    [Test]
    [DisplayName("Convertir un TenistaEntity en un Tenista")]
    public void ToTenistaFromEntity()
    {
        var testTenista = tenistaEntity.ToTenista();
        Assert.Multiple(() =>
        {
            Assert.That(testTenista.Id, Is.EqualTo(tenistaEntity.Id), "Id deben ser iguales");
            Assert.That(testTenista.Nombre, Is.EqualTo(tenistaEntity.Nombre), "Nombre deben ser iguales");
            Assert.That(testTenista.Pais, Is.EqualTo(tenistaEntity.Pais), "Pais deben ser iguales");
            Assert.That(testTenista.Altura, Is.EqualTo(tenistaEntity.Altura), "Altura deben ser iguales");
            Assert.That(testTenista.Peso, Is.EqualTo(tenistaEntity.Peso), "Peso deben ser iguales");
            Assert.That(testTenista.Puntos, Is.EqualTo(tenistaEntity.Puntos), "Puntos deben ser iguales");
        });
    }
}