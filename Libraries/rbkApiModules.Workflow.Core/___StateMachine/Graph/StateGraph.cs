﻿using Stateless.Reflection;

namespace Stateless.Graph
{
    /// <summary>
    /// This class is used to generate a symbolic representation of the
    /// graph structure, in preparation for feeding it to a diagram
    /// generator 
    /// </summary>
    public class StateGraph<TState, TTrigger>
    {
        private StateInfo<TState, TTrigger> initialState;

        /// <summary>
        /// List of all states in the graph, indexed by the string representation of the underlying State object.
        /// </summary>
        public Dictionary<string, State<TState, TTrigger>> States { get; private set; } = new Dictionary<string, State<TState, TTrigger>>();

        /// <summary>
        /// List of all transitions in the graph
        /// </summary>
        public List<Transition<TState, TTrigger>> Transitions { get; private set; } = new List<Transition<TState, TTrigger>>();

        /// <summary>
        /// List of all decision nodes in the graph.  A decision node is generated each time there
        /// is a PermitDynamic() transition.
        /// </summary>
        public List<Decision<TState, TTrigger>> Decisions { get; private set; } = new List<Decision<TState, TTrigger>>();

        /// <summary>
        /// Creates a new instance of <see cref="StateGraph"/>.
        /// </summary>
        /// <param name="machineInfo">An object which exposes the states, transitions, and actions of this machine.</param>
        public StateGraph(StateMachineInfo<TState, TTrigger> machineInfo)
        {
            // Add initial state
            initialState = machineInfo.InitialState;

            // Start with top-level superstates
            AddSuperstates(machineInfo);

            // Now add any states that aren't part of a tree
            AddSingleStates(machineInfo);

            // Now grab transitions
            AddTransitions(machineInfo);

            // Handle "OnEntryFrom"
            ProcessOnEntryFrom(machineInfo);
        }

        /// <summary>
        /// Convert the graph into a string representation, using the specified style.
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        public string ToGraph(GraphStyleBase<TState, TTrigger> style)
        {
            string dirgraphText = style.GetPrefix().Replace("\n", System.Environment.NewLine);

            // Start with the clusters
            foreach (var state in States.Values.Where(x => x is SuperState<TState, TTrigger>))
            {
                dirgraphText += style.FormatOneCluster((SuperState<TState, TTrigger>)state).Replace("\n", System.Environment.NewLine);
            }

            // Next process all non-cluster states
            foreach (var state in States.Values)
            {
                if ((state is SuperState<TState, TTrigger>) || (state is Decision<TState, TTrigger>) || (state.SuperState != null))
                    continue;
                dirgraphText += style.FormatOneState(state).Replace("\n", System.Environment.NewLine);
            }

            // Finally, add decision nodes
            foreach (var dec in Decisions)
            {
                dirgraphText += style.FormatOneDecisionNode(dec.NodeName, dec.Method.Description)
                    .Replace("\n", System.Environment.NewLine);
            }

            // now build behaviours
            List<string> transits = style.FormatAllTransitions(Transitions);
            foreach (var transit in transits)
                dirgraphText += System.Environment.NewLine + transit;

            // Add initial transition if present
            var initialStateName = initialState.UnderlyingState.ToString();
            dirgraphText += System.Environment.NewLine + $" init [label=\"\", shape=point];";
            dirgraphText += System.Environment.NewLine + $" init -> \"{initialStateName}\"[style = \"solid\"]";

            dirgraphText += System.Environment.NewLine + "}";

            return dirgraphText;
        }

        /// <summary>
        /// Process all entry actions that have a "FromTrigger" (meaning they are
        /// only executed when the state is entered because the specified trigger
        /// was fired).
        /// </summary>
        /// <param name="machineInfo"></param>
        void ProcessOnEntryFrom(StateMachineInfo<TState, TTrigger> machineInfo)
        {
            foreach (var stateInfo in machineInfo.States)
            {
                var state = States[stateInfo.UnderlyingState.ToString()];
                foreach (var entryAction in stateInfo.EntryActions)
                {
                    if (entryAction.FromTrigger != null)
                    {
                        // This 'state' has an 'entryAction' that only fires when it gets the trigger 'entryAction.FromTrigger'
                        // Does it have any incoming transitions that specify that trigger?
                        foreach (var transit in state.Arriving)
                        {
                            if ((transit.ExecuteEntryExitActions)
                                && (transit.Trigger.UnderlyingTrigger.ToString() == entryAction.FromTrigger))
                            {
                                transit.DestinationEntryActions.Add(entryAction);
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Add all transitions to the graph
        /// </summary>
        /// <param name="machineInfo"></param>
        void AddTransitions(StateMachineInfo<TState, TTrigger> machineInfo)
        {
            foreach (var stateInfo in machineInfo.States)
            {
                State<TState, TTrigger> fromState = States[stateInfo.UnderlyingState.ToString()];
                foreach (var fix in stateInfo.FixedTransitions)
                {
                    State<TState, TTrigger> toState = States[fix.DestinationState.UnderlyingState.ToString()];
                    if (fromState == toState)
                    {
                        var stay = new StayTransition<TState, TTrigger>(fromState, fix.Trigger, fix.GuardConditionsMethodDescriptions, true);
                        Transitions.Add(stay);
                        fromState.Leaving.Add(stay);
                        fromState.Arriving.Add(stay);
                    }
                    else
                    {
                        var trans = new FixedTransition<TState, TTrigger>(fromState, toState, fix.Trigger, fix.GuardConditionsMethodDescriptions);
                        Transitions.Add(trans);
                        fromState.Leaving.Add(trans);
                        toState.Arriving.Add(trans);
                    }
                }
                foreach (var dyno in stateInfo.DynamicTransitions)
                {
                    var decide = new Decision<TState, TTrigger>(dyno.DestinationStateSelectorDescription, Decisions.Count + 1);
                    Decisions.Add(decide);
                    var trans = new FixedTransition<TState, TTrigger>(fromState, decide, dyno.Trigger,
                        dyno.GuardConditionsMethodDescriptions);
                    Transitions.Add(trans);
                    fromState.Leaving.Add(trans);
                    decide.Arriving.Add(trans);
                    if (dyno.PossibleDestinationStates != null)
                    {
                        foreach (var dynamicStateInfo in dyno.PossibleDestinationStates)
                        {
                            States.TryGetValue(dynamicStateInfo.DestinationState, out State<TState, TTrigger> toState);
                            if (toState != null)
                            {
                                var dtrans = new DynamicTransition<TState, TTrigger>(decide, toState, dyno.Trigger, dynamicStateInfo.Criterion);
                                Transitions.Add(dtrans);
                                decide.Leaving.Add(dtrans);
                                toState.Arriving.Add(dtrans);
                            }
                        }
                    }
                }
                foreach (var igno in stateInfo.IgnoredTriggers)
                {
                    var stay = new StayTransition<TState, TTrigger>(fromState, igno.Trigger, igno.GuardConditionsMethodDescriptions, false);
                    Transitions.Add(stay);
                    fromState.Leaving.Add(stay);
                    fromState.Arriving.Add(stay);
                }
            }
        }


        /// <summary>
        /// Add states to the graph that are neither superstates, nor substates of a superstate.
        /// </summary>
        /// <param name="machineInfo"></param>
        void AddSingleStates(StateMachineInfo<TState, TTrigger> machineInfo)
        {
            foreach (var stateInfo in machineInfo.States)
            {
                if (!States.ContainsKey(stateInfo.UnderlyingState.ToString()))
                    States[stateInfo.UnderlyingState.ToString()] = new State<TState, TTrigger>(stateInfo);
            }
        }

        /// <summary>
        /// Add superstates to the graph (states that have substates)
        /// </summary>
        /// <param name="machineInfo"></param>
        void AddSuperstates(StateMachineInfo<TState, TTrigger> machineInfo)
        {
            foreach (var stateInfo in machineInfo.States.Where(sc => (sc.Substates?.Count() > 0) && (sc.Superstate == null)))
            {
                SuperState<TState, TTrigger> state = new SuperState<TState, TTrigger>(stateInfo);
                States[stateInfo.UnderlyingState.ToString()] = state;
                AddSubstates(state, stateInfo.Substates);
            }
        }

        void AddSubstates(SuperState<TState, TTrigger> superState, IEnumerable<StateInfo<TState, TTrigger>> substates)
        {
            foreach (var subState in substates)
            {
                if (States.ContainsKey(subState.UnderlyingState.ToString()))
                {
                    // This shouldn't happen
                }
                else if (subState.Substates.Any())
                {
                    SuperState<TState, TTrigger> sub = new SuperState<TState, TTrigger>(subState);
                    States[subState.UnderlyingState.ToString()] = sub;
                    superState.SubStates.Add(sub);
                    sub.SuperState = superState;
                    AddSubstates(sub, subState.Substates);
                }
                else
                {
                    var sub = new State<TState, TTrigger>(subState);
                    States[subState.UnderlyingState.ToString()] = sub;
                    superState.SubStates.Add(sub);
                    sub.SuperState = superState;
                }
            }
        }
    }
}
