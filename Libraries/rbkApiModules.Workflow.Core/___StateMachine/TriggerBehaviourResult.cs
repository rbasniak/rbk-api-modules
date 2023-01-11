namespace Stateless;

internal class TriggerBehaviourResult<TState, TTrigger>
{
    public TriggerBehaviourResult(TriggerBehaviour<TState, TTrigger> handler, ICollection<string> unmetGuardConditions)
    {
        Handler = handler;
        UnmetGuardConditions = unmetGuardConditions;
    }

    public TriggerBehaviour<TState, TTrigger> Handler { get; }

    public ICollection<string> UnmetGuardConditions { get; }
}
