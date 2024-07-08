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
    public void ToTenista()
    {
        var testTenista = tenistaDto.ToTenista();
        Assert.Multiple(() =>
        {
            Assert.AreEqual(tenista.Id, testTenista.Id, "Id deben ser iguales");
            Assert.AreEqual(tenista.Nombre, testTenista.Nombre, "Nombre deben ser iguales");
            Assert.AreEqual(tenista.Pais, testTenista.Pais, "Pais deben ser iguales");
            Assert.AreEqual(tenista.Altura, testTenista.Altura, "Altura deben ser iguales");
            Assert.AreEqual(tenista.Peso, testTenista.Peso, "Peso deben ser iguales");
            Assert.AreEqual(tenista.Puntos, testTenista.Puntos, "Puntos deben ser iguales");
            Assert.AreEqual(tenista.Mano, testTenista.Mano, "Mano deben ser iguales");
            Assert.AreEqual(tenista.FechaNacimiento, testTenista.FechaNacimiento, "FechaNacimiento deben ser iguales");
            Assert.AreEqual(tenista.IsDeleted, testTenista.IsDeleted, "IsDeleted deben ser iguales");
        });
    }

    [Test]
    public void ToTenistaDto()
    {
        var testDto = tenista.ToTenistaDto();
        Assert.Multiple(() =>
        {
            Assert.AreEqual(tenistaDto.Id, testDto.Id, "Id deben ser iguales");
            Assert.AreEqual(tenistaDto.Nombre, testDto.Nombre, "Nombre deben ser iguales");
            Assert.AreEqual(tenistaDto.Pais, testDto.Pais, "Pais deben ser iguales");
            Assert.AreEqual(tenistaDto.Altura, testDto.Altura, "Altura deben ser iguales");
            Assert.AreEqual(tenistaDto.Peso, testDto.Peso, "Peso deben ser iguales");
            Assert.AreEqual(tenistaDto.Puntos, testDto.Puntos, "Puntos deben ser iguales");
            Assert.AreEqual(tenistaDto.Mano, testDto.Mano, "Mano deben ser iguales");
            Assert.AreEqual(tenistaDto.FechaNacimiento, testDto.FechaNacimiento, "FechaNacimiento deben ser iguales");
            Assert.AreEqual(tenistaDto.IsDeleted, testDto.IsDeleted, "IsDeleted deben ser iguales");
        });
    }
}