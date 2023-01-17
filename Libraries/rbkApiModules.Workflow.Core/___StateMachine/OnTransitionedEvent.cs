namespace Stateless;

class OnTransitionedEvent<TState, TTrigger>
{
    event Action<Transition<TState, TTrigger>> _onTransitioned;
    readonly List<Func<Transition<TState, TTrigger>, Task>> _onTransitionedAsync = new List<Func<Transition<TState, TTrigger>, Task>>();

    public void Invoke(Transition<TState, TTrigger> transition)
    {
        if (_onTransitionedAsync.Count != 0)
            throw new InvalidOperationException(
                "Cannot execute asynchronous action specified as OnTransitioned callback. " +
                "Use asynchronous version of Fire [FireAsync]");

        _onTransitioned?.Invoke(transition);
    }

    public async Task InvokeAsync(Transition<TState, TTrigger> transition)
    {
        _onTransitioned?.Invoke(transition);

        foreach (var callback in _onTransitionedAsync)
            await callback(transition).ConfigureAwait(false);
    }

    public void Register(Action<Transition<TState, TTrigger>> action)
    {
        _onTransitioned += action;
    }

    public void Register(Func<Transition<TState, TTrigger>, Task> action)
    {
        _onTransitionedAsync.Add(action);
    }
}
