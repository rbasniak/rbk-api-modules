using System;
using System.Threading.Tasks;

namespace Stateless;

internal abstract class InternalTriggerBehaviour<TState, TTrigger> : TriggerBehaviour<TState, TTrigger>
{
    protected InternalTriggerBehaviour(TTrigger trigger, TransitionGuard guard) : base(trigger, guard)
    {
    }

    public abstract void Execute(Transition<TState, TTrigger> transition, object[] args);
    public abstract Task ExecuteAsync(Transition<TState, TTrigger> transition, object[] args);

    public override bool ResultsInTransitionFrom(TState source, object[] args, out TState destination)
    {
        destination = source;
        return false;
    }


    public class Sync : InternalTriggerBehaviour<TState, TTrigger>
    {
        public Action<Transition<TState, TTrigger>, object[]> InternalAction { get; }

        public Sync(TTrigger trigger, Func<object[], bool> guard, Action<Transition<TState, TTrigger>, object[]> internalAction, string guardDescription = null) : base(trigger, new TransitionGuard(guard, guardDescription))
        {
            InternalAction = internalAction;
        }
        public override void Execute(Transition<TState, TTrigger> transition, object[] args)
        {
            InternalAction(transition, args);
        }

        public override Task ExecuteAsync(Transition<TState, TTrigger> transition, object[] args)
        {
            Execute(transition, args);
            return TaskResult.Done;
        }
    }

    public class Async : InternalTriggerBehaviour<TState, TTrigger>
    {
        readonly Func<Transition<TState, TTrigger>, object[], Task> InternalAction;

        public Async(TTrigger trigger, NamedGuard guard, Func<Transition<TState, TTrigger>, object[], Task> internalAction) : base(trigger, new TransitionGuard(guard))
        {
            InternalAction = internalAction;
        }

        public override void Execute(Transition<TState, TTrigger> transition, object[] args)
        {
            throw new InvalidOperationException(
                $"Cannot execute asynchronous action specified in OnEntry event for '{transition.Destination}' state. " +
                 "Use asynchronous version of Fire [FireAsync]");
        }

        public override Task ExecuteAsync(Transition<TState, TTrigger> transition, object[] args)
        {
            return InternalAction(transition, args);
        }

    }
}