namespace CSharpLocalAndRemote.Notification;

public interface INotifications<T>
{
    Task Send(Notification<T> notification);
}