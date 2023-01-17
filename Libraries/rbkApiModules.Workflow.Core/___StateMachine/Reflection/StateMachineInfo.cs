namespace Stateless.Reflection
{
    /// <summary>
    /// An info object which exposes the states, transitions, and actions of this machine.
    /// </summary>
    public class StateMachineInfo<TState, TTrigger>
    {
        internal StateMachineInfo(IEnumerable<StateInfo<TState, TTrigger>> states, Type stateType, Type triggerType, StateInfo<TState, TTrigger> initialState)
        {
            InitialState = initialState;
            States = states?.ToList() ?? throw new ArgumentNullException(nameof(states));
            StateType = stateType;
            TriggerType = triggerType;
        }

        /// <summary>
        /// Exposes the initial state of this state machine.
        /// </summary>
        public StateInfo<TState, TTrigger> InitialState { get; }

        /// <summary>
        /// Exposes the states, transitions, and actions of this machine.
        /// </summary>
        public IEnumerable<StateInfo<TState, TTrigger>> States { get; }

        /// <summary>
        /// The type of the underlying state.
        /// </summary>
        /// <returns></returns>
        public Type StateType { get; private set; }

        /// <summary>
        /// The type of the underlying trigger.
        /// </summary>
        /// <returns></returns>
        public Type TriggerType { get; private set; }

    }
}
