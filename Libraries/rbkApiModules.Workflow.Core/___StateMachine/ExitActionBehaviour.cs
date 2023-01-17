namespace Stateless;

internal abstract class ExitActionBehavior<TState, TTrigger>
{
    public abstract void Execute(Transition<TState, TTrigger> transition);
    public abstract void Execute(Transition<TState, TTrigger> transition, object[] args);
    public abstract Task ExecuteAsync(Transition<TState, TTrigger> transition);
    public abstract Task ExecuteAsync(Transition<TState, TTrigger> transition, object[] args);

    protected ExitActionBehavior(Reflection.InvocationInfo actionDescription)
    {
        Description = actionDescription ?? throw new ArgumentNullException(nameof(actionDescription));
    }

    internal Reflection.InvocationInfo Description { get; }

    public class Sync : ExitActionBehavior<TState, TTrigger>
    {
        readonly Action<Transition<TState, TTrigger>, object[]> _action;

        public Sync(Action<Transition<TState, TTrigger>, object[]> action, Reflection.InvocationInfo actionDescription) : base(actionDescription)
        {
            _action = action;
        }

        public override void Execute(Transition<TState, TTrigger> transition, object[] args)
        {
            _action(transition, args);
        }

        public override void Execute(Transition<TState, TTrigger> transition)
        {
            _action(transition, null);
        }

        public override Task ExecuteAsync(Transition<TState, TTrigger> transition, object[] args)
        {
            Execute(transition, args);
            return TaskResult.Done;
        }

        public override Task ExecuteAsync(Transition<TState, TTrigger> transition)
        {
            Execute(transition);
            return TaskResult.Done;
        }
    }

    public class Async : ExitActionBehavior<TState, TTrigger>
    {
        readonly Func<Transition<TState, TTrigger>, object[], Task> _action;

        public Async(Func<Transition<TState, TTrigger>, object[], Task> action, Reflection.InvocationInfo actionDescription) : base(actionDescription)
        {
            _action = action;
        }

        public override void Execute(Transition<TState, TTrigger> transition, object[] args)
        {
            throw new InvalidOperationException(
                $"Cannot execute asynchronous action specified in OnExit event for '{transition.Source}' state. " +
                 "Use asynchronous version of Fire [FireAsync]");
        }

        public override void Execute(Transition<TState, TTrigger> transition)
        {
            throw new InvalidOperationException(
                $"Cannot execute asynchronous action specified in OnExit event for '{transition.Source}' state. " +
                 "Use asynchronous version of Fire [FireAsync]");
        }

        public override Task ExecuteAsync(Transition<TState, TTrigger> transition, object[] args)
        {
            return _action(transition, args);
        }

        public override Task ExecuteAsync(Transition<TState, TTrigger> transition)
        {
            return _action(transition, null);
        }
    }
}
