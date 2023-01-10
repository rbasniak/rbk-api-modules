using Microsoft.EntityFrameworkCore;
using rbkApiModules.Notifications.Core;

namespace rbkApiModules.Notifications.Relational;

public class RelationalNotificationsService : INotificationsService
{
    private readonly DbContext _context;

    public RelationalNotificationsService(DbContext context)
    {
        _context = context;
    }

    public async Task<Notification[]> GetAllAsync(string username, string category, NotificationStatus? status, NotificationType? type, CancellationToken cancellation = default)
    {
        var notifications = await _context.Set<Notification>()
           .Where(x => x.User.ToLower() == username.ToLower() &&
                (String.IsNullOrEmpty(category) || x.Category == category) &&
                (status == null || x.Status == status) &&
                (type == null || x.Type == type))
           .OrderByDescending(x => x.Date)
           .ToArrayAsync(cancellation);

        return notifications;
    }

    public async Task<Notification> CreateAsync(string username, string category, string title, string body, string route, string link, NotificationType type, CancellationToken cancellation = default)
    {
        var notification = new Notification(category, title, body, link, route, link, type);

        await _context.AddAsync(notification, cancellation);

        await _context.SaveChangesAsync(cancellation);

        return notification;
    }
    
    public async Task UpdateStatusAsync(string username, Guid[] notificationIds, NotificationStatus status, CancellationToken cancellation = default)
    {
        var notifications = await _context.Set<Notification>()
                    .Where(x => x.User.ToLower() == username &&
                        notificationIds.Any(y => x.Id == y)).ToListAsync(cancellation);

        foreach (var notification in notifications)
        {
            notification.SetStatus(status);
        }

        await _context.SaveChangesAsync(cancellation);
    }

    public async Task DeleteAsync(string username, Guid[] notificationIds, CancellationToken cancellation = default)
    {
        var notifications = await _context.Set<Notification>()
            .Where(x => x.User.ToLower() == username.ToLower() &&
                notificationIds.Any(y => x.Id == y)).ToListAsync(cancellation);

        _context.RemoveRange(notifications);

        await _context.SaveChangesAsync(cancellation);
    }
}