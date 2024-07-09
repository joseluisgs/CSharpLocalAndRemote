using System.ComponentModel;
using CSharpLocalAndRemote.model;
using CSharpLocalAndRemote.Validator;

namespace Tests.Validator;

[TestFixture]
public class TenistaValidatorTest
{
    [Test]
    [DisplayName("Validar Tenista con nombre vacío devuelve error de validación")]
    public void Validar_TenistaConNombreVacio_DevuelveErrorDeValidacion()
    {
        // Arrange
        var tenista = new Tenista("", "Suiza", 185, 85, 2000, Mano.DIESTRO, new DateTime(1981, 8, 8), id: 1);

        // Act
        var resultado = tenista.Validate();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(resultado.IsFailure, "El resultado debe ser un fallo");
            Assert.That(resultado.Error.ToString(), Is.EqualTo("ERROR: El nombre es requerido"),
                "El mensaje de error debe ser 'ERROR: El nombre es requerido'");
        });
    }

    [Test]
    [DisplayName("Validar Tenista con altura menor o igual a 0 devuelve error de validación")]
    public void Validar_TenistaConAlturaMenorOIgualACero_DevuelveErrorDeValidacion()
    {
        // Arrange
        var tenista = new Tenista("Juan", "España", 0, 75, 1000, Mano.DIESTRO, new DateTime(1990, 5, 22), id: 2);

        // Act
        var resultado = tenista.Validate();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(resultado.IsFailure, Is.True, "El resultado debe ser un fallo");
            Assert.That(resultado.Error.ToString(), Is.EqualTo("ERROR: La altura debe ser mayor a 0"),
                "El mensaje de error debe ser 'ERROR: La altura debe ser mayor a 0'");
        });
    }

    [Test]
    [DisplayName("Validar Tenista con peso menor a 0 devuelve error de validación")]
    public void Validar_TenistaConPesoMenorACero_DevuelveErrorDeValidacion()
    {
        // Arrange
        var tenista = new Tenista("Juan", "España", 180, 0, 1000, Mano.DIESTRO, new DateTime(1990, 5, 22), id: 3);

        // Act
        var resultado = tenista.Validate();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(resultado.IsFailure, Is.True, "El resultado debe ser un fallo");
            Assert.That(resultado.Error.ToString(), Is.EqualTo("ERROR: El peso debe ser mayor o igual a 0"),
                "El mensaje de error debe ser 'ERROR: El peso debe ser mayor o igual a 0'");
        });
    }

    [Test]
    [DisplayName("Validar Tenista con puntos menor a 0 devuelve error de validación")]
    public void Validar_TenistaConPuntosMenorACero_DevuelveErrorDeValidacion()
    {
        // Arrange
        var tenista = new Tenista("Juan", "España", 180, 75, -1, Mano.DIESTRO, new DateTime(1990, 5, 2), id: 4);

        // Act
        var resultado = tenista.Validate();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(resultado.IsFailure, Is.True, "El resultado debe ser un fallo");
            Assert.That(resultado.Error.ToString(), Is.EqualTo("ERROR: Los puntos deben ser mayor o igual a 0"),
                "El mensaje de error debe ser 'ERROR: Los puntos deben ser mayor o igual a 0'");
        });
    }

    [Test]
    [DisplayName("Validar Tenista con fecha de nacimiento futura devuelve error de validación")]
    public void Validar_TenistaConFechaNacimientoFutura_DevuelveErrorDeValidacion()
    {
        // Arrange
        var tenista = new Tenista("Juan", "España", 180, 75, 1000, Mano.DIESTRO, new DateTime(2025, 5, 22), id: 5);

        // Act
        var resultado = tenista.Validate();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(resultado.IsFailure, Is.True, "El resultado debe ser un fallo");
            Assert.That(resultado.Error.ToString(),
                Is.EqualTo("ERROR: La fecha de nacimiento no puede ser mayor a la fecha actual"),
                "El mensaje de error debe ser 'ERROR: La fecha de nacimiento no puede ser mayor a la fecha actual'");
        });
    }
}