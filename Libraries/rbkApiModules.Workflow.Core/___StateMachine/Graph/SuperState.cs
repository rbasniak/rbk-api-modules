using System.Collections.Generic;

namespace Stateless.Graph
{
    /// <summary>
    /// Used to keep track of a state that has substates
    /// </summary>
    public class SuperState<TState, TTrigger> : State<TState, TTrigger>
    {
        /// <summary>
        /// List of states that are a substate of this state
        /// </summary>
        public List<State<TState, TTrigger>> SubStates { get; } = new List<State<TState, TTrigger>>();

        /// <summary>
        /// Constructs a new instance of SuperState.
        /// </summary>
        /// <param name="stateInfo">The super state to be represented.</param>
        public SuperState(Reflection.StateInfo<TState, TTrigger> stateInfo)
            : base(stateInfo)
        {

        }
    }
}
