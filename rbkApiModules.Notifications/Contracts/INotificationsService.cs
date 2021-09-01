using Microsoft.EntityFrameworkCore;
using rbkApiModules.Notifications.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rbkApiModules.Notifications
{
    public interface INotificationsService
    {
        Task<List<Notification>> GetNotifications(string userKey, string catgory, NotificationStatus? status, NotificationType? type);

        Task<Notification> Create(string userKey, string category, string title, string body, string route, string link, NotificationType type);
    }

    public class NotificationsService : INotificationsService
    {
        private readonly DbContext _context;

        public NotificationsService(DbContext context)
        {
            _context = context;
        }

        public async Task<List<Notification>> GetNotifications(string userKey, string category, NotificationStatus? status, NotificationType? type)
        {
            var notifications = await _context.Set<Notification>()
               .Where(x => x.User.ToUpper() == userKey.ToUpper() &&
                    (string.IsNullOrEmpty(category) || x.Category == category) &&
                    (status == null || x.Status == status) &&
                    (type == null || x.Type == type))
               .OrderByDescending(x => x.Date)
               .ToListAsync();

            return notifications;
        }

        public async Task<Notification> Create(string userKey, string category, string title, string body, string route, string link, NotificationType type)
        {
            var notification = _context.Add(new Notification(category, title, body, link, route, link, type));

            await _context.SaveChangesAsync();

            return notification.Entity;
        }
    }
}
