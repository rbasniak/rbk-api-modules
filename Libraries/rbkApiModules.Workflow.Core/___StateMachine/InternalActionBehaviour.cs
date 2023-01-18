namespace Stateless;

internal abstract class InternalActionBehaviour<TState, TTrigger>
{
    public abstract void Execute(Transition<TState, TTrigger> transition);
    public abstract Task ExecuteAsync(Transition<TState, TTrigger> transition);

    public class Sync : InternalActionBehaviour<TState, TTrigger>
    {
        readonly Action<Transition<TState, TTrigger>> _action;

        public Sync(Action<Transition<TState, TTrigger>> action)
        {
            _action = action;
        }

        public override void Execute(Transition<TState, TTrigger> transition)
        {
            _action(transition);
        }

        public override Task ExecuteAsync(Transition<TState, TTrigger> transition)
        {
            Execute(transition);
            return TaskResult.Done;
        }
    }

    public class Async : InternalActionBehaviour<TState, TTrigger>
    {
        readonly Func<Transition<TState, TTrigger>, Task> _action;

        public Async(Func<Transition<TState, TTrigger>, Task> action)
        {
            _action = action;
        }

        public override void Execute(Transition<TState, TTrigger> transition)
        {
            throw new InvalidOperationException(
                $"Cannot execute asynchronous action specified in OnEntry event for '{transition.Destination}' state. " +
                    "Use asynchronous version of Fire [FireAsync]");
        }

        public override Task ExecuteAsync(Transition<TState, TTrigger> transition)
        {
            return _action(transition);
        }
    }
}
