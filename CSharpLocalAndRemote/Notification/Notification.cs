namespace CSharpLocalAndRemote.Notification;

public record Notification<T>(
    NotificationType Type,
    T? Item,
    string? Message,
    DateTime CreatedAt
);

public enum NotificationType
{
    Created,
    Updated,
    Deleted,
    Refresh
}