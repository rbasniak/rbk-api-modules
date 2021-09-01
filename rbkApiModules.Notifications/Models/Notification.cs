using rbkApiModules.Infrastructure.Models;
using rbkApiModules.Notifications.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rbkApiModules.Notifications
{
    /// <summary>
    /// Classe representando notificações
    /// </summary>
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

        /// <summary>
        /// Data da notificação
        /// </summary>
        public virtual DateTime Date { get; private set; }

        /// <summary>
        /// Categoria da notificação
        /// </summary>
        public virtual string Category { get; private set; }

        /// <summary>
        /// Título da notificação
        /// </summary>
        public virtual string Title { get; private set; }

        /// <summary>
        /// Corpo da notificação
        /// </summary>
        public virtual string Body { get; private set; }

        /// <summary>
        /// Rota para o qual a notificação vai redirecionar ao clicar nela.
        /// </summary>
        public virtual string Route { get; private set; }

        /// <summary>
        /// Link externo para onde a notificação vai redirecionar ao clicar nela
        /// </summary>
        public virtual string Link { get; private set; }

        /// <summary>
        /// Estado da notificação
        /// </summary>
        public NotificationStatus Status { get; private set; }

        /// <summary>
        /// Tipo da notificação
        /// </summary>
        public virtual NotificationType Type { get; private set; }

        /// <summary>
        /// Usuário a quem essa notificação pertence
        /// </summary>
        public virtual string User { get; private set; }

        public void SetStatus(NotificationStatus status)
        {
            Status = status;
        }
    }
}
