﻿namespace Stateless;

internal abstract class ActivateActionBehaviour<TState, TTrigger>
{
    readonly TState _state;

    protected ActivateActionBehaviour(TState state, Reflection.InvocationInfo actionDescription)
    {
        _state = state;
        Description = actionDescription ?? throw new ArgumentNullException(nameof(actionDescription));
    }

    internal Reflection.InvocationInfo Description { get; }

    public abstract void Execute();
    public abstract Task ExecuteAsync();

    public class Sync : ActivateActionBehaviour<TState, TTrigger>
    {
        readonly Action _action;

        public Sync(TState state, Action action, Reflection.InvocationInfo actionDescription)
            : base(state, actionDescription)
        {
            _action = action;
        }

        public override void Execute()
        {
            _action();
        }

        public override Task ExecuteAsync()
        {
            Execute();
            return TaskResult.Done;
        }
    }

    public class Async : ActivateActionBehaviour<TState, TTrigger>
    {
        readonly Func<Task> _action;

        public Async(TState state, Func<Task> action, Reflection.InvocationInfo actionDescription)
            : base(state, actionDescription)
        {
            _action = action;
        }

        public override void Execute()
        {
            throw new InvalidOperationException(
                $"Cannot execute asynchronous action specified in OnActivateAsync for '{_state}' state. " +
                 "Use asynchronous version of Activate [ActivateAsync]");
        }

        public override Task ExecuteAsync()
        {
            return _action();
        }
    }
}
