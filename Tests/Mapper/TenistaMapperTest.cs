using System.ComponentModel;
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
        Mano.DIESTRO,
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
        "DIESTRO",
        "1986-06-03",
        IsDeleted: false
    );

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
            Assert.That(testDto.Mano, Is.EqualTo(tenistaDto.Mano), "Mano deben ser iguales");
            Assert.That(testDto.FechaNacimiento, Is.EqualTo(tenistaDto.FechaNacimiento),
                "FechaNacimiento deben ser iguales");
            Assert.That(testDto.IsDeleted, Is.EqualTo(tenistaDto.IsDeleted), "IsDeleted deben ser iguales");
        });
    }
}