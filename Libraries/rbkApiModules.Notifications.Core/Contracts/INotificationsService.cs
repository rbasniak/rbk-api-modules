namespace rbkApiModules.Notifications.Core;

public interface INotificationsService
{
    Task<Notification[]> GetAllAsync(string username, string catgory, NotificationStatus? status, NotificationType? type, CancellationToken cancellation = default);
    Task<Notification> CreateAsync(string username, string category, string title, string body, string route, string link, NotificationType type, CancellationToken cancellation = default);
    Task UpdateStatusAsync(string username, Guid[] notificationIds, NotificationStatus status, CancellationToken cancellation = default);
    Task DeleteAsync(string username, Guid[] notificationIds, CancellationToken cancellation = default);
}
