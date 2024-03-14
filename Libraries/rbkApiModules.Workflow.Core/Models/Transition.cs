using rbkApiModules.Commons.Core;

namespace rbkApiModules.Workflow.Core;

public class Transition: BaseEntity
{
    protected Transition()
    {

    }

    public Transition(Guid fromStateId, Guid eventId, Guid toStateId, string history, bool isProtected, bool isActive)
    {
        FromStateId = fromStateId;
        ToStateId = toStateId;
        EventId = eventId;
        History = history;
        IsProtected = isProtected;
        IsActive = isActive;
    }

    public Transition(State fromState, Event @event, State toState, string history, bool isProtected, bool isActive)
    {
        FromState = fromState;
        ToState = toState;
        Event = @event;
        History = history;
        IsProtected = isProtected;
        IsActive = isActive;
    }

    public virtual bool IsActive { get; protected set; }

    /// <summary>
    /// State ao qual essa transição pertence
    /// </summary>
    public virtual Guid FromStateId { get; protected set; }
    public virtual State FromState { get; protected set; }

    /// <summary>
    /// State para o qual essa transição leva
    /// </summary>
    public virtual Guid ToStateId { get; protected set; }
    public virtual State ToState { get; protected set; }

    /// <summary>
    /// Evento que dispara essa transição
    /// </summary>
    public virtual Guid EventId { get; protected set; }
    public virtual Event Event { get; protected set; }

    /// <summary>
    /// Flag que indica se é possível ou não apagar essa transição
    /// </summary>
    public virtual bool IsProtected { get; protected set; }

    /// <summary>
    /// Texto para ser exibido no histórico de operações da requisição
    /// </summary>
    public virtual string History { get; protected set; }

    public virtual void Update(Event @event, State toState, string history)
    {
        Event = @event;
        ToState = toState;
        History = history;
    }

    public override string ToString()
    {
        return $"{FromState.Name} -> {Event.Name} -> {ToState.Name}";
    }
}
