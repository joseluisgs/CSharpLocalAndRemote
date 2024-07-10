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
        [DisplayName("Enviar notificación se hace cuando el canal no está lleno")]
        public async Task EnviarNotificacion_CuandoElCanalNoEstaLleno()
        {
            // Arrange
            var tenistaTest = new Tenista("Test1", "Suiza", 185, 85, 2000, Mano.Diestro, new DateTime(1981, 8, 8), id: 1)
                .ToTenistaDto();
            var notification = new Notification<TenistaDto>(NotificationType.Created, tenistaTest, "Test", DateTime.Now);
            var tenistasNotifications = new TenistasNotifications();

            Notification<TenistaDto> result = null;

            // Act
            var subscription = tenistasNotifications.Notifications.Subscribe(n => result = n);
            await tenistasNotifications.Send(notification);
            //await Task.Delay(500); // Pequeña espera para asegurar la recepción de la notificación

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(notification), "La notificación enviada debe ser igual a la recibida.");
                Assert.That(result.Type, Is.EqualTo(notification.Type), "El tipo de notificación debe ser igual.");
                Assert.That(result.Message, Is.EqualTo(notification.Message), "El mensaje de la notificación debe ser igual.");
                Assert.That(result.Item, Is.EqualTo(notification.Item), "El item de la notificación debe ser igual.");
            });

            subscription.Dispose();
        }

        [Test]
        [DisplayName("Enviar notificación descarta la antigua y recibe la nueva")]
        public async Task EnviarNotificacion_DescartaViejaYRecibeNueva()
        {
            // Arrange
            var tenistaTest = new Tenista("Test1", "Suiza", 185, 85, 2000, Mano.Diestro, new DateTime(1981, 8, 8), id: 1);

            var notification1 = new Notification<TenistaDto>(NotificationType.Created, tenistaTest.ToTenistaDto(), "Test1", DateTime.Now);
            var notification2 = new Notification<TenistaDto>(NotificationType.Created, tenistaTest.ToTenistaDto(), "Test2", DateTime.Now);

            var tenistasNotifications = new TenistasNotifications();

            Notification<TenistaDto> result = null;

            // Act
            var subscription = tenistasNotifications.Notifications.Subscribe(n => result = n);
            await tenistasNotifications.Send(notification1);
            await tenistasNotifications.Send(notification2);
            //await Task.Delay(500); // Pequeña espera para asegurar la recepción de la notificación

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(notification2), "La notificación 2 enviada debe ser igual a la recibida.");
                Assert.That(result.Type, Is.EqualTo(notification2.Type), "El tipo de notificación debe ser igual.");
                Assert.That(result.Message, Is.EqualTo(notification2.Message), "El mensaje de la notificación debe ser igual.");
                Assert.That(result.Item, Is.EqualTo(notification2.Item), "El item de la notificación debe ser igual.");
            });

            subscription.Dispose();
        }
}