using rbkApiModules.Commons.Core;

namespace rbkApiModules.Workflow.Core;

public abstract class BaseStateEntity : BaseEntity
{
    protected HashSet<StateChangeEvent> _events;

    protected BaseStateEntity()
    {

    }

    protected BaseStateEntity(State state)
    {
        _events = new HashSet<StateChangeEvent>();

        State = state;
    }

    public virtual Guid StateId { get; protected set; }
    public virtual State State { get; protected set; }

    public virtual IEnumerable<StateChangeEvent> Events => _events?.ToList();

    public virtual void NextStatus(Event trigger, string user)
    {
        if (State == null)
        {
            throw new ArgumentNullException(nameof(State), "You need to load the State navigation property");
        }

        if (Events == null)
        {
            throw new ArgumentNullException(nameof(Events), "You need to load the Events list property");
        }

        if (State.Transitions == null)
        {
            throw new ArgumentNullException(nameof(State.Transitions), "You need to load the Transitions list from the State navigation property");
        }

        foreach (var child in State.Transitions)
        {
            if (child.FromState == null) throw new ArgumentNullException(nameof(child.FromState), "Transition not fully loaded from database");
            if (child.ToState == null) throw new ArgumentNullException(nameof(child.ToState), "Transition not fully loaded from database");
            if (child.Event == null) throw new ArgumentNullException(nameof(child.Event), "Transition not fully loaded from database");
        }

        Transition transition = null;

        var transitionHistory = String.Empty;

        transition = State.Transitions.FirstOrDefault(x => x.Event.Id == trigger.Id);

        if (transition != null)
        {
            State = transition.ToState;
            transitionHistory = transition.History;

            ExecuteDomainSpecificActions(user, transition);
        }
        else
        {
            throw new SafeException("Mudança de status inválida para a entidade selecionada");
        }

        var @event = CreateStateChangeEvent(user, transition);
        _events.Add(@event);
    }

    protected virtual void ExecuteDomainSpecificActions(string user, Transition transition)
    {

    }

    protected abstract StateChangeEvent CreateStateChangeEvent(string user, Transition transition); 
}
