using System;
using System.Threading.Tasks;

namespace Stateless;

internal abstract class EntryActionBehavior<TState, TTrigger>
{
    protected EntryActionBehavior(Reflection.InvocationInfo description)
    {
        Description = description;
    }

    public Reflection.InvocationInfo Description { get; }

    public abstract void Execute(Transition<TState, TTrigger> transition, object[] args);
    public abstract Task ExecuteAsync(Transition<TState, TTrigger> transition, object[] args);

    public class Sync : EntryActionBehavior<TState, TTrigger>
    {
        readonly Action<Transition<TState, TTrigger>, object[]> _action;

        public Sync(Action<Transition<TState, TTrigger>, object[]> action, Reflection.InvocationInfo description) : base(description)
        {
            _action = action;
        }

        public override void Execute(Transition<TState, TTrigger> transition, object[] args)
        {
            _action(transition, args);
        }

        public override Task ExecuteAsync(Transition <TState, TTrigger>transition, object[] args)
        {
            Execute(transition, args);
            return TaskResult.Done;
        }
    }

    public class SyncFrom<TTriggerType> : Sync
    {
        internal TTriggerType Trigger { get; private set; }

        public SyncFrom(TTriggerType trigger, Action<Transition<TState, TTrigger>, object[]> action, Reflection.InvocationInfo description)
            : base(action, description)
        {
            Trigger = trigger;
        }

        public override void Execute(Transition<TState, TTrigger> transition, object[] args)
        {
            if (transition.Trigger.Equals(Trigger))
                base.Execute(transition, args);
        }

        public override Task ExecuteAsync(Transition<TState, TTrigger> transition, object[] args)
        {
            Execute(transition, args);
            return TaskResult.Done;
        }
    }

    public class Async : EntryActionBehavior<TState, TTrigger>
    {
        readonly Func<Transition<TState, TTrigger>, object[], Task> _action;

        public Async(Func<Transition<TState, TTrigger>, object[], Task> action, Reflection.InvocationInfo description) : base(description)
        {
            _action = action;
        }

        public override void Execute(Transition<TState, TTrigger> transition, object[] args)
        {
            throw new InvalidOperationException(
                $"Cannot execute asynchronous action specified in OnEntry event for '{transition.Destination}' state. " +
                 "Use asynchronous version of Fire [FireAsync]");
        }

        public override Task ExecuteAsync(Transition<TState, TTrigger> transition, object[] args)
        {
            return _action(transition, args);
        }
    }
}
