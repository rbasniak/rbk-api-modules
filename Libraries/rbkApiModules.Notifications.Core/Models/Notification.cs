using rbkApiModules.Commons.Core;

namespace rbkApiModules.Notifications.Core;

public class Notification : BaseEntity
{
    protected Notification()
    {
    }

    public Notification(string category, string title, string body, string route, string link, string user, NotificationType type)
    {
        Date = DateTime.UtcNow;
        Status = NotificationStatus.New;

        Title = title;
        Category = category;
        Body = body;
        Type = type;
        Link = link;
        Route = route;
        User = user;
    }

    public virtual DateTime Date { get; private set; }

    public virtual string Category { get; private set; }

    public virtual string Title { get; private set; }

    public virtual string Body { get; private set; }

    public virtual string Route { get; private set; }

    public virtual string Link { get; private set; }

    public virtual NotificationStatus Status { get; private set; }

    public virtual NotificationType Type { get; private set; }

    public virtual string User { get; private set; }

    public void SetStatus(NotificationStatus status)
    {
        Status = status;
    }
}
