using System.Threading.Channels;
using CSharpLocalAndRemote.Dto;
using CSharpLocalAndRemote.Logger;

namespace CSharpLocalAndRemote.Notification;

public class TenistasNotifications : INotifications<TenistaDto>
{
    // Para poder cancelar la suscripción a las notificaciones
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly Serilog.Core.Logger _logger = LoggerUtils<TenistasNotifications>.GetLogger();

    // Canal de notificaciones
    private readonly Channel<Notification<TenistaDto>> _notificationsChannel;

    public TenistasNotifications()
    {
        // Creamos un canal de notificaciones con capacidad de 1 elemento y modo de eliminación de los más antiguos si se llena
        var boundedOptions = new BoundedChannelOptions(1) { FullMode = BoundedChannelFullMode.DropOldest };

        _notificationsChannel = Channel.CreateBounded<Notification<TenistaDto>>(boundedOptions); // Creamos el canal
        _cancellationTokenSource = new CancellationTokenSource(); // Creamos el token de cancelación
    }

    // Propiedad de solo lectura para obtener el canal de notificaciones
    public ChannelReader<Notification<TenistaDto>> Notifications => _notificationsChannel.Reader;
    public CancellationToken CancellationToken => _cancellationTokenSource.Token;

    public async Task Send(Notification<TenistaDto> notification)
    {
        _logger.Debug("Enviando notificación: {Notification}", notification);
        await _notificationsChannel.Writer.WriteAsync(notification);
    }

    public void Stop()
    {
        _logger.Debug("Deteniendo notificaciones...");
        _notificationsChannel.Writer.Complete(); // Indicamos que no se van a escribir más notificaciones
        _cancellationTokenSource.Cancel(); // Cancelamos la suscripción
    }
}