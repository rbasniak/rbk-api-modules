using Stateless.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stateless
{
    /// <summary>
    /// Models behaviour as transitions between a finite set of states.
    /// </summary>
    /// <typeparam name="TState">The type used to represent the states.</typeparam>
    /// <typeparam name="TTrigger">The type used to represent the triggers that cause state transitions.</typeparam>
    public partial class StateMachine<TState, TTrigger>
    {
        private readonly IDictionary<TState, StateRepresentation<TState, TTrigger>> _stateConfiguration = new Dictionary<TState, StateRepresentation<TState, TTrigger>>();
        private readonly Func<TState> _stateAccessor;
        private readonly Action<TState> _stateMutator;
        private UnhandledTriggerAction<TState, TTrigger> _unhandledTriggerAction;
        private readonly OnTransitionedEvent<TState, TTrigger> _onTransitionedEvent;
        private readonly OnTransitionedEvent<TState, TTrigger> _onTransitionCompletedEvent;
        private readonly TState _initialState;
        private readonly FiringMode _firingMode;

        private readonly Queue<QueuedTrigger<TTrigger>> _eventQueue = new Queue<QueuedTrigger<TTrigger>>();
        private bool _firing;

        /// <summary>
        /// Construct a state machine with external state storage.
        /// </summary>
        /// <param name="stateAccessor">A function that will be called to read the current state value.</param>
        /// <param name="stateMutator">An action that will be called to write new state values.</param>
        public StateMachine(Func<TState> stateAccessor, Action<TState> stateMutator) :this(stateAccessor, stateMutator, FiringMode.Queued)
        {
        }

        /// <summary>
        /// Construct a state machine.
        /// </summary>
        /// <param name="initialState">The initial state.</param>
        public StateMachine(TState initialState) : this(initialState, FiringMode.Queued)
        {
        }

        /// <summary>
        /// Construct a state machine with external state storage.
        /// </summary>
        /// <param name="stateAccessor">A function that will be called to read the current state value.</param>
        /// <param name="stateMutator">An action that will be called to write new state values.</param>
        /// <param name="firingMode">Optional specification of fireing mode.</param>
        public StateMachine(Func<TState> stateAccessor, Action<TState> stateMutator, FiringMode firingMode) : this()
        {
            _stateAccessor = stateAccessor ?? throw new ArgumentNullException(nameof(stateAccessor));
            _stateMutator = stateMutator ?? throw new ArgumentNullException(nameof(stateMutator));

            _initialState = stateAccessor();
            _firingMode = firingMode;
        }

        /// <summary>
        /// Construct a state machine.
        /// </summary>
        /// <param name="initialState">The initial state.</param>
        /// <param name="firingMode">Optional specification of fireing mode.</param>
        public StateMachine(TState initialState, FiringMode firingMode) : this()
        {
            var reference = new StateReference<TState> { State = initialState };
            _stateAccessor = () => reference.State;
            _stateMutator = s => reference.State = s;

            _initialState = initialState;
            _firingMode = firingMode;
        }


        /// <summary>
        /// Default constructor
        /// </summary>
        StateMachine()
        {
            _unhandledTriggerAction = new UnhandledTriggerAction<TState, TTrigger>.Sync(DefaultUnhandledTriggerAction);
            _onTransitionedEvent = new OnTransitionedEvent<TState, TTrigger>();
            _onTransitionCompletedEvent = new OnTransitionedEvent<TState, TTrigger>();
        }

        /// <summary>
        /// The current state.
        /// </summary>
        public TState State
        {
            get
            {
                return _stateAccessor();
            }
            private set
            {
                _stateMutator(value);
            }
        }

        /// <summary>
        /// The currently-permissible trigger values.
        /// </summary>
        public IEnumerable<TTrigger> PermittedTriggers
        {
            get
            {
                return GetPermittedTriggers();
            }
        }

        /// <summary>
        /// The currently-permissible trigger values.
        /// </summary>
        public IEnumerable<TTrigger> GetPermittedTriggers(params object[] args)
        {
            return CurrentRepresentation.GetPermittedTriggers(args);
        }

        StateRepresentation<TState, TTrigger> CurrentRepresentation
        {
            get
            {
                return GetRepresentation(State);
            }
        }

        public IEnumerable<TTrigger> GetDetailedPermittedTriggers(params object[] args)
        {
            return CurrentRepresentation.GetPermittedTriggers(args);
        }

        /// <summary>
        /// Provides an info object which exposes the states, transitions, and actions of this machine.
        /// </summary>
        public StateMachineInfo<TState, TTrigger> GetInfo()
        {
            var initialState = StateInfo<TState, TTrigger>.CreateStateInfo(new StateRepresentation<TState, TTrigger>(_initialState));

            var representations = _stateConfiguration.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var behaviours = _stateConfiguration.SelectMany(kvp => kvp.Value.TriggerBehaviours.SelectMany(b => b.Value.OfType<TransitioningTriggerBehaviour<TState, TTrigger>>().Select(tb => tb.Destination))).ToList();
            behaviours.AddRange(_stateConfiguration.SelectMany(kvp => kvp.Value.TriggerBehaviours.SelectMany(b => b.Value.OfType<ReentryTriggerBehaviour<TState, TTrigger>>().Select(tb => tb.Destination))).ToList());

            var reachable = behaviours
                .Distinct()
                .Except(representations.Keys)
                .Select(underlying => new StateRepresentation<TState, TTrigger>(underlying))
                .ToArray();

            foreach (var representation in reachable)
                representations.Add(representation.UnderlyingState, representation);

            var info = representations.ToDictionary(kvp => kvp.Key, kvp => StateInfo<TState, TTrigger>.CreateStateInfo(kvp.Value));

            foreach (var state in info)
                StateInfo<TState, TTrigger>.AddRelationships(state.Value, representations[state.Key], k => info[k]);

            return new StateMachineInfo<TState, TTrigger>(info.Values, typeof(TState), typeof(TTrigger), initialState);
        }

        StateRepresentation<TState, TTrigger> GetRepresentation(TState state)
        {
            if (!_stateConfiguration.TryGetValue(state, out StateRepresentation<TState, TTrigger> result))
            {
                result = new StateRepresentation<TState, TTrigger>(state);
                _stateConfiguration.Add(state, result);
            }

            return result;
        }

        /// <summary>
        /// Begin configuration of the entry/exit actions and allowed transitions
        /// when the state machine is in a particular state.
        /// </summary>
        /// <param name="state">The state to configure.</param>
        /// <returns>A configuration object through which the state can be configured.</returns>
        public StateConfiguration<TState, TTrigger> Configure(TState state)
        {
            return new StateConfiguration<TState, TTrigger>(this, GetRepresentation(state), GetRepresentation);
        }

        /// <summary>
        /// Transition from the current state via the specified trigger.
        /// The target state is determined by the configuration of the current state.
        /// Actions associated with leaving the current state and entering the new one
        /// will be invoked.
        /// </summary>
        /// <param name="trigger">The trigger to fire.</param>
        /// <exception cref="System.InvalidOperationException">The current state does
        /// not allow the trigger to be fired.</exception>
        public void Fire(TTrigger trigger)
        {
            InternalFire(trigger, new object[0]);
        }

        /// <summary>
        /// Transition from the current state via the specified trigger.
        /// The target state is determined by the configuration of the current state.
        /// Actions associated with leaving the current state and entering the new one
        /// will be invoked.
        /// </summary>
        /// <param name="trigger">The trigger to fire.</param>
        /// <param name="args">A variable-length parameters list containing arguments. </param>
        /// <exception cref="System.InvalidOperationException">The current state does
        /// not allow the trigger to be fired.</exception>
        public void Fire(TTrigger trigger, params object[] args)
        {
            if (trigger == null) throw new ArgumentNullException(nameof(trigger));
            InternalFire(trigger, args);
        } 

        /// <summary>
        /// Activates current state. Actions associated with activating the current state
        /// will be invoked. The activation is idempotent and subsequent activation of the same current state
        /// will not lead to re-execution of activation callbacks.
        /// </summary>
        public void Activate()
        {
            var representativeState = GetRepresentation(State);
            representativeState.Activate();
        }

        /// <summary>
        /// Deactivates current state. Actions associated with deactivating the current state
        /// will be invoked. The deactivation is idempotent and subsequent deactivation of the same current state
        /// will not lead to re-execution of deactivation callbacks.
        /// </summary>
        public void Deactivate()
        {
            var representativeState = GetRepresentation(State);
            representativeState.Deactivate();
        }

        /// <summary>
        /// Determine how to Fire the trigger
        /// </summary>
        /// <param name="trigger">The trigger. </param>
        /// <param name="args">A variable-length parameters list containing arguments. </param>
        void InternalFire(TTrigger trigger, params object[] args)
        {
            switch (_firingMode)
            {
                case FiringMode.Immediate:
                    InternalFireOne(trigger, args);
                    break;
                case FiringMode.Queued:
                    InternalFireQueued(trigger, args);
                    break;
                default:
                    // If something is completely messed up we let the user know ;-)
                    throw new InvalidOperationException("The firing mode has not been configured!");
            }
        }

        /// <summary>
        /// Queue events and then fire in order.
        /// If only one event is queued, this behaves identically to the non-queued version.
        /// </summary>
        /// <param name="trigger">  The trigger. </param>
        /// <param name="args">     A variable-length parameters list containing arguments. </param>
        private void InternalFireQueued(TTrigger trigger, params object[] args)
        {
            // Add trigger to queue
            _eventQueue.Enqueue(new QueuedTrigger<TTrigger> { Trigger = trigger, Args = args });

            // If a trigger is already being handled then the trigger will be queued (FIFO) and processed later.
            if (_firing)
            {
                return;
            }

            try
            {
                _firing = true;

                // Empty queue for triggers
                while (_eventQueue.Any())
                {
                    var queuedEvent = _eventQueue.Dequeue();
                    InternalFireOne(queuedEvent.Trigger, queuedEvent.Args);
                }
            }
            finally
            {
                _firing = false;
            }
        }

        /// <summary>
        /// This method handles the execution of a trigger handler. It finds a
        /// handle, then updates the current state information.
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="args"></param>
        void InternalFireOne(TTrigger trigger, params object[] args)
        {
            var source = State;
            var representativeState = GetRepresentation(source);

            // Try to find a trigger handler, either in the current state or a super state.
            if (!representativeState.TryFindHandler(trigger, args, out TriggerBehaviourResult<TState, TTrigger> result))
            {
                _unhandledTriggerAction.Execute(representativeState.UnderlyingState, trigger, result?.UnmetGuardConditions);
                return;
            }

            switch (result.Handler)
            {
                // Check if this trigger should be ignored
                case IgnoredTriggerBehaviour<TState, TTrigger> _:
                    return;
                // Handle special case, re-entry in superstate
                // Check if it is an internal transition, or a transition from one state to another.
                case ReentryTriggerBehaviour<TState, TTrigger> handler:
                {
                    // Handle transition, and set new state
                    var transition = new Transition<TState, TTrigger>(source, handler.Destination, trigger, args);
                    HandleReentryTrigger(args, representativeState, transition);
                    break;
                }
                case DynamicTriggerBehaviour<TState, TTrigger> _ when (result.Handler.ResultsInTransitionFrom(source, args, out var destination)):
                case TransitioningTriggerBehaviour<TState, TTrigger> _ when (result.Handler.ResultsInTransitionFrom(source, args, out destination)):
                {
                    // Handle transition, and set new state
                    var transition = new Transition<TState, TTrigger>(source, destination, trigger, args);
                    HandleTransitioningTrigger(args, representativeState, transition);

                    break;
                }
                case InternalTriggerBehaviour<TState, TTrigger> _:
                {
                    // Internal transitions does not update the current state, but must execute the associated action.
                    var transition = new Transition<TState, TTrigger>(source, source, trigger, args);
                    CurrentRepresentation.InternalAction(transition, args);
                    break;
                }
                default:
                    throw new InvalidOperationException("State machine configuration incorrect, no handler for trigger.");
            }
        }

        private void HandleReentryTrigger(object[] args, StateRepresentation<TState, TTrigger> representativeState, Transition<TState, TTrigger> transition)
        {
            StateRepresentation<TState, TTrigger> representation;
            transition = representativeState.Exit(transition);
            var newRepresentation = GetRepresentation(transition.Destination);

            if (!transition.Source.Equals(transition.Destination))
            {
                // Then Exit the final superstate
                transition = new Transition<TState, TTrigger>(transition.Destination, transition.Destination, transition.Trigger, args);
                newRepresentation.Exit(transition);

                _onTransitionedEvent.Invoke(transition);
                representation = EnterState(newRepresentation, transition, args);
                _onTransitionCompletedEvent.Invoke(transition);

            }
            else
            {
                _onTransitionedEvent.Invoke(transition);
                representation = EnterState(newRepresentation, transition, args);
                _onTransitionCompletedEvent.Invoke(transition);
            }
            State = representation.UnderlyingState;
        }

        private void HandleTransitioningTrigger( object[] args, StateRepresentation<TState, TTrigger> representativeState, Transition<TState, TTrigger> transition)
        {
            transition = representativeState.Exit(transition);

            State = transition.Destination;
            var newRepresentation = GetRepresentation(transition.Destination);

            //Alert all listeners of state transition
            _onTransitionedEvent.Invoke(transition);
            var representation = EnterState(newRepresentation, transition, args);

            // Check if state has changed by entering new state (by fireing triggers in OnEntry or such)
            if (!representation.UnderlyingState.Equals(State))
            {
                // The state has been changed after entering the state, must update current state to new one
                State = representation.UnderlyingState;
            }

            _onTransitionCompletedEvent.Invoke(new Transition<TState, TTrigger>(transition.Source, State, transition.Trigger, transition.Parameters));
        }

        private StateRepresentation<TState, TTrigger> EnterState(StateRepresentation<TState, TTrigger> representation, Transition<TState, TTrigger> transition, object [] args)
        {
            // Enter the new state
            representation.Enter(transition, args);

            if (FiringMode.Immediate.Equals(_firingMode) && !State.Equals(transition.Destination))
            {
                // This can happen if triggers are fired in OnEntry
                // Must update current representation with updated State
                representation = GetRepresentation(State);
                transition = new Transition<TState, TTrigger>(transition.Source, State, transition.Trigger, args);
            }

            // Recursively enter substates that have an initial transition
            if (representation.HasInitialTransition)
            {
                // Verify that the target state is a substate
                // Check if state has substate(s), and if an initial transition(s) has been set up.
                if (!representation.GetSubstates().Any(s => s.UnderlyingState.Equals(representation.InitialTransitionTarget)))
                {
                    throw new InvalidOperationException($"The target ({representation.InitialTransitionTarget}) for the initial transition is not a substate.");
                }

                var initialTransition = new InitialTransition<TState, TTrigger>(transition.Source, representation.InitialTransitionTarget, transition.Trigger, args);
                representation = GetRepresentation(representation.InitialTransitionTarget);

                // Alert all listeners of initial state transition
                _onTransitionedEvent.Invoke(new Transition<TState, TTrigger>(transition.Destination, initialTransition.Destination, transition.Trigger, transition.Parameters));
                representation = EnterState(representation, initialTransition, args);
            }

            return representation;
        }

        /// <summary>
        /// Override the default behaviour of throwing an exception when an unhandled trigger
        /// is fired.
        /// </summary>
        /// <param name="unhandledTriggerAction">An action to call when an unhandled trigger is fired.</param>
        public void OnUnhandledTrigger(Action<TState, TTrigger> unhandledTriggerAction)
        {
            if (unhandledTriggerAction == null) throw new ArgumentNullException(nameof(unhandledTriggerAction));
            _unhandledTriggerAction = new UnhandledTriggerAction<TState, TTrigger>.Sync((s, t, c) => unhandledTriggerAction(s, t));
        }

        /// <summary>
        /// Override the default behaviour of throwing an exception when an unhandled trigger
        /// is fired.
        /// </summary>
        /// <param name="unhandledTriggerAction">An action to call when an unhandled trigger is fired.</param>
        public void OnUnhandledTrigger(Action<TState, TTrigger, ICollection<string>> unhandledTriggerAction)
        {
            if (unhandledTriggerAction == null) throw new ArgumentNullException(nameof(unhandledTriggerAction));
            _unhandledTriggerAction = new UnhandledTriggerAction<TState, TTrigger>.Sync(unhandledTriggerAction);
        }

        /// <summary>
        /// Determine if the state machine is in the supplied state.
        /// </summary>
        /// <param name="state">The state to test for.</param>
        /// <returns>True if the current state is equal to, or a substate of,
        /// the supplied state.</returns>
        public bool IsInState(TState state)
        {
            return CurrentRepresentation.IsIncludedIn(state);
        }

        /// <summary>
        /// Returns true if <paramref name="trigger"/> can be fired
        /// in the current state.
        /// </summary>
        /// <param name="trigger">Trigger to test.</param>
        /// <returns>True if the trigger can be fired, false otherwise.</returns>
        public bool CanFire(TTrigger trigger)
        {
            return CurrentRepresentation.CanHandle(trigger);
        }

        /// <summary>
        /// Returns true if <paramref name="trigger"/> can be fired
        /// in the current state.
        /// </summary>
        /// <param name="trigger">Trigger to test.</param>
        /// <param name="unmetGuards">Guard descriptions of unmet guards. If given trigger is not configured for current state, this will be null.</param>
        /// <returns>True if the trigger can be fired, false otherwise.</returns>
        public bool CanFire(TTrigger trigger, out ICollection<string> unmetGuards)
        {
            return CurrentRepresentation.CanHandle(trigger, new object[] { }, out unmetGuards);
        }

        /// <summary>
        /// A human-readable representation of the state machine.
        /// </summary>
        /// <returns>A description of the current state and permitted triggers.</returns>
        public override string ToString()
        {
            return string.Format(
                "StateMachine {{ State = {0}, PermittedTriggers = {{ {1} }}}}",
                State,
                string.Join(", ", GetPermittedTriggers().Select(t => t.ToString()).ToArray()));
        } 

        void DefaultUnhandledTriggerAction(TState state, TTrigger trigger, ICollection<string> unmetGuardConditions)
        {
            if (unmetGuardConditions?.Any() ?? false)
                throw new InvalidOperationException(
                    string.Format(
                        "Trigger '{0}' is valid for transition from state '{1}' but a guard conditions are not met. Guard descriptions: '{2}'.",
                        trigger, state, string.Join(", ", unmetGuardConditions)));

            throw new InvalidOperationException(
                string.Format(
                    "No valid leaving transitions are permitted from state '{1}' for trigger '{0}'. Consider ignoring the trigger.",
                    trigger, state));
        }

        /// <summary>
        /// Registers a callback that will be invoked every time the state machine
        /// transitions from one state into another.
        /// </summary>
        /// <param name="onTransitionAction">The action to execute, accepting the details
        /// of the transition.</param>
        public void OnTransitioned(Action<Transition<TState, TTrigger>> onTransitionAction)
        {
            if (onTransitionAction == null) throw new ArgumentNullException(nameof(onTransitionAction));
            _onTransitionedEvent.Register(onTransitionAction);
        }

        /// <summary>
        /// Registers a callback that will be invoked every time the statemachine
        /// transitions from one state into another and all the OnEntryFrom etc methods
        /// have been invoked
        /// </summary>
        /// <param name="onTransitionAction">The action to execute, accepting the details
        /// of the transition.</param>
        public void OnTransitionCompleted(Action<Transition<TState, TTrigger>> onTransitionAction)
        {
            if (onTransitionAction == null) throw new ArgumentNullException(nameof(onTransitionAction));
            _onTransitionCompletedEvent.Register(onTransitionAction);
        }











        /// <summary>
        /// Activates current state in asynchronous fashion. Actions associated with activating the currrent state
        /// will be invoked. The activation is idempotent and subsequent activation of the same current state 
        /// will not lead to re-execution of activation callbacks.
        /// </summary>
        public Task ActivateAsync()
        {
            var representativeState = GetRepresentation(State);
            return representativeState.ActivateAsync();
        }

        /// <summary>
        /// Deactivates current state in asynchronous fashion. Actions associated with deactivating the currrent state
        /// will be invoked. The deactivation is idempotent and subsequent deactivation of the same current state 
        /// will not lead to re-execution of deactivation callbacks.
        /// </summary>
        public Task DeactivateAsync()
        {
            var representativeState = GetRepresentation(State);
            return representativeState.DeactivateAsync();
        }

        /// <summary>
        /// Transition from the current state via the specified trigger in async fashion.
        /// The target state is determined by the configuration of the current state.
        /// Actions associated with leaving the current state and entering the new one
        /// will be invoked.
        /// </summary>
        /// <param name="trigger">The trigger to fire.</param>
        /// <exception cref="System.InvalidOperationException">The current state does
        /// not allow the trigger to be fired.</exception>
        public Task FireAsync(TTrigger trigger)
        {
            return InternalFireAsync(trigger, new object[0]);
        }

        /// <summary>
        /// Transition from the current state via the specified trigger in async fashion.
        /// The target state is determined by the configuration of the current state.
        /// Actions associated with leaving the current state and entering the new one
        /// will be invoked.
        /// </summary>
        /// <typeparam name="TArg0">Type of the first trigger argument.</typeparam>
        /// <param name="trigger">The trigger to fire.</param>
        /// <param name="arg0">The first argument.</param>
        /// <exception cref="System.InvalidOperationException">The current state does
        /// not allow the trigger to be fired.</exception>
        public Task FireAsync(TTrigger trigger, object[] args)
        {
            return InternalFireAsync(trigger, args);
        }

        /// <summary>
        /// Determine how to Fire the trigger
        /// </summary>
        /// <param name="trigger">The trigger. </param>
        /// <param name="args">A variable-length parameters list containing arguments. </param>
        async Task InternalFireAsync(TTrigger trigger, params object[] args)
        {
            switch (_firingMode)
            {
                case FiringMode.Immediate:
                    await InternalFireOneAsync(trigger, args);
                    break;
                case FiringMode.Queued:
                    await InternalFireQueuedAsync(trigger, args);
                    break;
                default:
                    // If something is completely messed up we let the user know ;-)
                    throw new InvalidOperationException("The firing mode has not been configured!");
            }
        }

        /// <summary>
        /// Queue events and then fire in order.
        /// If only one event is queued, this behaves identically to the non-queued version.
        /// </summary>
        /// <param name="trigger">  The trigger. </param>
        /// <param name="args">     A variable-length parameters list containing arguments. </param>
        async Task InternalFireQueuedAsync(TTrigger trigger, params object[] args)
        {
            if (_firing)
            {
                _eventQueue.Enqueue(new QueuedTrigger<TTrigger> { Trigger = trigger, Args = args });
                return;
            }

            try
            {
                _firing = true;

                await InternalFireOneAsync(trigger, args).ConfigureAwait(false);

                while (_eventQueue.Count != 0)
                {
                    var queuedEvent = _eventQueue.Dequeue();
                    await InternalFireOneAsync(queuedEvent.Trigger, queuedEvent.Args).ConfigureAwait(false);
                }
            }
            finally
            {
                _firing = false;
            }
        }

        async Task InternalFireOneAsync(TTrigger trigger, params object[] args)
        {
            var source = State;
            var representativeState = GetRepresentation(source);

            // Try to find a trigger handler, either in the current state or a super state.
            if (!representativeState.TryFindHandler(trigger, args, out TriggerBehaviourResult<TState, TTrigger> result))
            {
                await _unhandledTriggerAction.ExecuteAsync(representativeState.UnderlyingState, trigger, result?.UnmetGuardConditions);
                return;
            }

            switch (result.Handler)
            {
                // Check if this trigger should be ignored
                case IgnoredTriggerBehaviour<TState, TTrigger> _:
                    return;
                // Handle special case, re-entry in superstate
                // Check if it is an internal transition, or a transition from one state to another.
                case ReentryTriggerBehaviour<TState, TTrigger> handler:
                    {
                        // Handle transition, and set new state
                        var transition = new Transition<TState, TTrigger>(source, handler.Destination, trigger, args);
                        await HandleReentryTriggerAsync(args, representativeState, transition);
                        break;
                    }
                case DynamicTriggerBehaviour<TState, TTrigger> _ when (result.Handler.ResultsInTransitionFrom(source, args, out var destination)):
                case TransitioningTriggerBehaviour<TState, TTrigger> _ when (result.Handler.ResultsInTransitionFrom(source, args, out destination)):
                    {
                        // Handle transition, and set new state
                        var transition = new Transition<TState, TTrigger>(source, destination, trigger, args);
                        await HandleTransitioningTriggerAsync(args, representativeState, transition);

                        break;
                    }
                case InternalTriggerBehaviour<TState, TTrigger> itb:
                    {
                        // Internal transitions does not update the current state, but must execute the associated action.
                        var transition = new Transition<TState, TTrigger>(source, source, trigger, args);

                        if (itb is InternalTriggerBehaviour<TState, TTrigger>.Async ita)
                            await ita.ExecuteAsync(transition, args);
                        else
                            await Task.Run(() => itb.Execute(transition, args));
                        break;
                    }
                default:
                    throw new InvalidOperationException("State machine configuration incorrect, no handler for trigger.");
            }
        }

        private async Task HandleReentryTriggerAsync(object[] args, StateRepresentation<TState, TTrigger> representativeState, Transition<TState, TTrigger> transition)
        {
            StateRepresentation<TState, TTrigger> representation;
            transition = await representativeState.ExitAsync(transition);
            var newRepresentation = GetRepresentation(transition.Destination);

            if (!transition.Source.Equals(transition.Destination))
            {
                // Then Exit the final superstate
                transition = new Transition<TState, TTrigger>(transition.Destination, transition.Destination, transition.Trigger, args);
                await newRepresentation.ExitAsync(transition);

                await _onTransitionedEvent.InvokeAsync(transition);
                representation = await EnterStateAsync(newRepresentation, transition, args);
                await _onTransitionCompletedEvent.InvokeAsync(transition);
            }
            else
            {
                await _onTransitionedEvent.InvokeAsync(transition);
                representation = await EnterStateAsync(newRepresentation, transition, args);
                await _onTransitionCompletedEvent.InvokeAsync(transition);
            }
            State = representation.UnderlyingState;
        }

        private async Task HandleTransitioningTriggerAsync(object[] args, StateRepresentation<TState, TTrigger> representativeState, Transition<TState, TTrigger> transition)
        {
            transition = await representativeState.ExitAsync(transition);

            State = transition.Destination;
            var newRepresentation = GetRepresentation(transition.Destination);

            //Alert all listeners of state transition
            await _onTransitionedEvent.InvokeAsync(transition);
            var representation = await EnterStateAsync(newRepresentation, transition, args);

            // Check if state has changed by entering new state (by fireing triggers in OnEntry or such)
            if (!representation.UnderlyingState.Equals(State))
            {
                // The state has been changed after entering the state, must update current state to new one
                State = representation.UnderlyingState;
            }

            await _onTransitionCompletedEvent.InvokeAsync(new Transition<TState, TTrigger>(transition.Source, State, transition.Trigger, transition.Parameters));
        }


        private async Task<StateRepresentation<TState, TTrigger>> EnterStateAsync(StateRepresentation<TState, TTrigger> representation, Transition<TState, TTrigger> transition, object[] args)
        {
            // Enter the new state
            await representation.EnterAsync(transition, args);

            if (FiringMode.Immediate.Equals(_firingMode) && !State.Equals(transition.Destination))
            {
                // This can happen if triggers are fired in OnEntry
                // Must update current representation with updated State
                representation = GetRepresentation(State);
                transition = new Transition<TState, TTrigger>(transition.Source, State, transition.Trigger, args);
            }

            // Recursively enter substates that have an initial transition
            if (representation.HasInitialTransition)
            {
                // Verify that the target state is a substate
                // Check if state has substate(s), and if an initial transition(s) has been set up.
                if (!representation.GetSubstates().Any(s => s.UnderlyingState.Equals(representation.InitialTransitionTarget)))
                {
                    throw new InvalidOperationException($"The target ({representation.InitialTransitionTarget}) for the initial transition is not a substate.");
                }

                var initialTransition = new InitialTransition<TState, TTrigger>(transition.Source, representation.InitialTransitionTarget, transition.Trigger, args);
                representation = GetRepresentation(representation.InitialTransitionTarget);

                // Alert all listeners of initial state transition
                await _onTransitionedEvent.InvokeAsync(new Transition<TState, TTrigger>(transition.Destination, initialTransition.Destination, transition.Trigger, transition.Parameters));
                representation = await EnterStateAsync(representation, initialTransition, args);
            }

            return representation;
        }

        /// <summary>
        /// Override the default behaviour of throwing an exception when an unhandled trigger
        /// is fired.
        /// </summary>
        /// <param name="unhandledTriggerAction"></param>
        public void OnUnhandledTriggerAsync(Func<TState, TTrigger, Task> unhandledTriggerAction)
        {
            if (unhandledTriggerAction == null) throw new ArgumentNullException(nameof(unhandledTriggerAction));
            _unhandledTriggerAction = new UnhandledTriggerAction<TState, TTrigger>.Async((s, t, c) => unhandledTriggerAction(s, t));
        }

        /// <summary>
        /// Override the default behaviour of throwing an exception when an unhandled trigger
        /// is fired.
        /// </summary>
        /// <param name="unhandledTriggerAction">An asynchronous action to call when an unhandled trigger is fired.</param>
        public void OnUnhandledTriggerAsync(Func<TState, TTrigger, ICollection<string>, Task> unhandledTriggerAction)
        {
            if (unhandledTriggerAction == null) throw new ArgumentNullException(nameof(unhandledTriggerAction));
            _unhandledTriggerAction = new UnhandledTriggerAction<TState, TTrigger>.Async(unhandledTriggerAction);
        }

        /// <summary>
        /// Registers an asynchronous callback that will be invoked every time the statemachine
        /// transitions from one state into another.
        /// </summary>
        /// <param name="onTransitionAction">The asynchronous action to execute, accepting the details
        /// of the transition.</param>
        public void OnTransitionedAsync(Func<Transition<TState, TTrigger>, Task> onTransitionAction)
        {
            if (onTransitionAction == null) throw new ArgumentNullException(nameof(onTransitionAction));
            _onTransitionedEvent.Register(onTransitionAction);
        }

        /// <summary>
        /// Registers a callback that will be invoked every time the statemachine
        /// transitions from one state into another and all the OnEntryFrom etc methods
        /// have been invoked
        /// </summary>
        /// <param name="onTransitionAction">The asynchronous action to execute, accepting the details
        /// of the transition.</param>
        public void OnTransitionCompletedAsync(Func<Transition<TState, TTrigger>, Task> onTransitionAction)
        {
            if (onTransitionAction == null) throw new ArgumentNullException(nameof(onTransitionAction));
            _onTransitionCompletedEvent.Register(onTransitionAction);
        }
    }
}
