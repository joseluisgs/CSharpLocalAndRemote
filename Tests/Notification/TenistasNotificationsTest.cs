using System.ComponentModel;
using System.Threading.Channels;
using CSharpLocalAndRemote.Dto;
using CSharpLocalAndRemote.Mapper;
using CSharpLocalAndRemote.model;
using CSharpLocalAndRemote.Notification;

namespace Tests.Notification;

[TestFixture]
[TestOf(typeof(TenistasNotifications))]
public class TenistasNotificationsTest
{
    [Test]
    [DisplayName("EnviarNotificacion se hace cuando el canal no está lleno")]
    public async Task EnviarNotificacion_CuandoElCanalNoEstaLleno()
    {
        // Arrange
        var tenistaTest = new Tenista("Test1", "Suiza", 185, 85, 2000, Mano.Diestro, new DateTime(1981, 8, 8), id: 1)
            .ToTenistaDto();

        var notification = new Notification<TenistaDto>(NotificationType.Created, tenistaTest, "Test", DateTime.Now);

        var tenistasNotifications = new TenistasNotifications();

        // Act
        await tenistasNotifications.Send(notification);
        var result = await tenistasNotifications.Notifications.ReadAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(notification), "La notificación enviada debe ser igual a la recibida.");
            Assert.That(result.Type, Is.EqualTo(notification.Type), "El tipo de notificación debe ser igual.");
            Assert.That(result.Message, Is.EqualTo(notification.Message),
                "El mensaje de la notificación debe ser igual.");
            Assert.That(result.Item, Is.EqualTo(notification.Item),
                "El item de la notificación debe ser igual.");
        });
    }

    [Test]
    [DisplayName("EnviarNotificacion no se hace cuando el canal no está lleno")]
    public async Task EnviarNotificacion_NoSeHaceCuandoElCanalEstaLleno()
    {
        // Arrange
        var tenistaTest = new Tenista("Test1", "Suiza", 185, 85, 2000, Mano.Diestro, new DateTime(1981, 8, 8), id: 1);

        var notification1 = new Notification<TenistaDto>(NotificationType.Created, tenistaTest.ToTenistaDto(), "Test1",
            DateTime.Now);
        var notification2 = new Notification<TenistaDto>(NotificationType.Created, tenistaTest.ToTenistaDto(), "Test2",
            DateTime.Now);

        var tenistasNotifications = new TenistasNotifications();

        // Act
        await tenistasNotifications.Send(notification1);
        await tenistasNotifications.Send(notification2);

        var result1 = await tenistasNotifications.Notifications.ReadAsync();


        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result1, Is.EqualTo(notification2),
                "La notificación 2 enviada debe ser igual a la recibida.");
            Assert.That(result1.Type, Is.EqualTo(notification2.Type), "El tipo de notificación debe ser igual.");
            Assert.That(result1.Message, Is.EqualTo(notification2.Message),
                "El mensaje de la notificación debe ser igual.");
            Assert.That(result1.Item, Is.EqualTo(notification2.Item), "El item de la notificación debe ser igual.");
        });
    }

    [Test]
    [DisplayName("CancelarSuscripcion detiene las notificaciones")]
    public void CancelarSuscripcion_DetenerNotificaciones()
    {
        // Arrange
        var tenistaTest = new Tenista("Test1", "Suiza", 185, 85, 2000, Mano.Diestro, new DateTime(1981, 8, 8), id: 1)
            .ToTenistaDto();
        var tenistasNotifications = new TenistasNotifications();

        // Act
        tenistasNotifications.Stop();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.ThrowsAsync<ChannelClosedException>(
                async () => await tenistasNotifications.Send(
                    new Notification<TenistaDto>(NotificationType.Created, tenistaTest, "Test", DateTime.Now)),
                "Debería lanzarse una excepción al intentar enviar a un canal cerrado.");
            Assert.IsTrue(tenistasNotifications.CancellationToken.IsCancellationRequested,
                "El token de cancelación debería estar establecido.");
        });
    }
}