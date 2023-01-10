namespace Stateless;

internal class IgnoredTriggerBehaviour<TState, TTrigger> : TriggerBehaviour<TState, TTrigger>
{
    public IgnoredTriggerBehaviour(TTrigger trigger, TransitionGuard transitionGuard)
        : base(trigger, transitionGuard)
    {
    }

    public override bool ResultsInTransitionFrom(TState source, object[] args, out TState destination)
    {
        destination = default(TState);
        return false;
    }
}
