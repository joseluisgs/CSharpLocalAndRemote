using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpLocalAndRemote.Dto;
using CSharpLocalAndRemote.Logger;

namespace CSharpLocalAndRemote.Notification;

public class TenistasNotifications : ITenistasNotifications
{
    // Voy con la implementación de RxNet
    private readonly Serilog.Core.Logger _logger = LoggerUtils<TenistasNotifications>.GetLogger();

    // BehaviorSubject es un tipo de flujo como Mono de Project Reactor
    // que mantiene un valor un solo valor. En este caso, mantiene una notificación
    // Por muchos suscriptores que haya, siempre se les enviará el último valor
    private readonly BehaviorSubject<Notification<TenistaDto>?> _notificationsSubject = new(null); // Inicialmente null

    // Propiedad para obtener el flujo de notificaciones, pero solo las que no son null
    public IObservable<Notification<TenistaDto>?> GetNotifications()
    {
        return _notificationsSubject.AsObservable()
            .Skip(1);
        // Saltamos el primer null, tambien podemos usar Where(x => x != null)
    }


    public Task Send(Notification<TenistaDto> notification)
    {
        _logger.Debug("Enviando notificación: {Notification}", notification);
        _notificationsSubject.OnNext(notification);
        return Task.CompletedTask;
    }

    public void Stop()
    {
        _logger.Debug("Deteniendo notificaciones...");
        _notificationsSubject.OnCompleted();
    }
}