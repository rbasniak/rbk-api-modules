using rbkApiModules.Commons.Core;

namespace rbkApiModules.Workflow.Core;

public class StateChangeEvent : BaseEntity
{
    protected StateChangeEvent()
    {

    }

    public StateChangeEvent(BaseStateEntity entity, string username, string statusName, string historyText, string notes)
    {
        Entity = entity;
        Username = username;
        Date = DateTime.Now;
        StatusHistory = historyText;
        StatusName = statusName;
        Notes = notes;
    }

    /// <summary>
    /// Texto com o nome do state que originou esse evento, no momento em que ele aconteceu
    /// </summary>
    public virtual string StatusName { get; protected set; }

    /// <summary>
    /// Texto para ser exibido no histórico da solicitação
    /// </summary>
    public virtual string StatusHistory { get; protected set; }

    /// <summary>
    /// Chave do usário que provocou a mudança de estado
    /// </summary>
    public virtual string Username { get; protected set; }

    public virtual DateTime Date { get; protected set; }

    /// <summary>
    /// Solicitação de mudança à qual este evento pertence
    /// </summary>
    public virtual Guid EntityId { get; protected set; }
    public virtual BaseStateEntity Entity { get; protected set; }

    public virtual string Notes { get; protected set; }

    public override string ToString()
    {
        return $"{Date.ToString()} - {StatusName}";
    }
}
