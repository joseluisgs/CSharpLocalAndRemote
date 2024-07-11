using CSharpLocalAndRemote.Dto;

namespace CSharpLocalAndRemote.Notification;

public interface ITenistasNotifications : INotifications<TenistaDto>
{
    IObservable<Notification<TenistaDto>?> GetNotifications();
}