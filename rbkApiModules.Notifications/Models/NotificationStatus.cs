using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Notifications.Models
{
    /// <summary>
    /// Possíveis estados das notificações
    /// </summary>
    public enum NotificationStatus
    {
        /// <summary>
        /// Nova. O usuário ainda não visualizou.
        /// </summary>
        New,

        /// <summary>
        /// Notificado. O usuário já foi notificado mas ainda não 
        /// viu os dados relacionado à notificação.
        /// </summary>
        Notified,

        /// <summary>
        /// Visualizada. O usuário já visualizou.
        /// </summary>
        Viewed,

        /// <summary>
        /// Arquivado. O usuário moveu manualmente a notificação
        /// para a pasta de arquivados.
        /// </summary>
        Archived,
    }
}
