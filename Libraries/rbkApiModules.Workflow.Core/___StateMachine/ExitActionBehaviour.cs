using System;
using System.Threading.Tasks;

namespace Stateless;

internal abstract class ExitActionBehavior<TState, TTrigger>
{
    public abstract void Execute(Transition<TState, TTrigger> transition);
    public abstract Task ExecuteAsync(Transition<TState, TTrigger> transition);

    protected ExitActionBehavior(Reflection.InvocationInfo actionDescription)
    {
        Description = actionDescription ?? throw new ArgumentNullException(nameof(actionDescription));
    }

    internal Reflection.InvocationInfo Description { get; }

    public class Sync : ExitActionBehavior<TState, TTrigger>
    {
        readonly Action<Transition<TState, TTrigger>> _action;

        public Sync(Action<Transition<TState, TTrigger>> action, Reflection.InvocationInfo actionDescription) : base(actionDescription)
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

    public class Async : ExitActionBehavior<TState, TTrigger>
    {
        readonly Func<Transition<TState, TTrigger>, Task> _action;

        public Async(Func<Transition<TState, TTrigger>, Task> action, Reflection.InvocationInfo actionDescription) : base(actionDescription)
        {
            _action = action;
        }

        public override void Execute(Transition<TState, TTrigger> transition)
        {
            throw new InvalidOperationException(
                $"Cannot execute asynchronous action specified in OnExit event for '{transition.Source}' state. " +
                 "Use asynchronous version of Fire [FireAsync]");
        }

        public override Task ExecuteAsync(Transition<TState, TTrigger> transition)
        {
            return _action(transition);
        }
    }
}
