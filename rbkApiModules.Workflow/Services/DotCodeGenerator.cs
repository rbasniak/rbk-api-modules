using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Workflow.Services
{
    public static class DotCodeGenerator
    {
        public static string GenerateCode<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup, TClaimToQueryDefinition>(List<TState> states)
            where TState : BaseState<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup, TClaimToQueryDefinition>
            where TEvent : BaseEvent<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup, TClaimToQueryDefinition>
            where TTransition : BaseTransition<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup, TClaimToQueryDefinition>
            where TStateEntity : BaseStateEntity<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup, TClaimToQueryDefinition>
            where TClaimToEvent : BaseClaimToEvent<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup, TClaimToQueryDefinition>
            where TStateChangeEvent : BaseStateChangeEvent<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup, TClaimToQueryDefinition>
            where TStateGroup : BaseStateGroup<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup, TClaimToQueryDefinition>
            where TQueryDefinitionGroup : BaseQueryDefinitionGroup<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup, TClaimToQueryDefinition>
            where TQueryDefinition : BaseQueryDefinition<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup, TClaimToQueryDefinition>
            where TQueryDefinitionToState : BaseQueryDefinitionToState<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup, TClaimToQueryDefinition>, new()
            where TQueryDefinitionToGroup : BaseQueryDefinitionToGroup<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup, TClaimToQueryDefinition>
            where TClaimToQueryDefinition : BaseClaimToQueryDefinition<TState, TEvent, TTransition, TStateEntity, TClaimToEvent, TStateChangeEvent, TStateGroup, TQueryDefinitionGroup, TQueryDefinition, TQueryDefinitionToState, TQueryDefinitionToGroup, TClaimToQueryDefinition>
        {
            var code = new StringBuilder();

            code.AppendLine("digraph finite_state_machine {");
            code.AppendLine($"    rankdir=TD;");

            var initialStates = states.Where(x => x.UsedBy.Count() == 0).ToList();
            var intermediateStates = states.Where(x => x.Transitions.Count() > 0 && x.UsedBy.Count() > 0).ToList();
            var finalStates = states.Where(x => x.Transitions.Count() == 0).ToList();

            code.AppendLine($"");
            code.AppendLine($"    node [shape=oval style=filled color=black fillcolor=\"#ffff99\"]");

            foreach (var state in initialStates)
            {
                code.AppendLine($"        {state.Id.ToDotId()} [label = \"{state.Name}\"];");
            }

            code.AppendLine($"");
            code.AppendLine($"    node [shape=rectangle style=\"filled, rounded\" color=black fillcolor=\"#beaed4\"]");
            foreach (var state in finalStates)
            {
                code.AppendLine($"        {state.Id.ToDotId()} [label = \"{state.Name}\"];");
            }

            code.AppendLine($"");
            code.AppendLine($"    node [shape=rectangle style=\"filled, rounded\" color=black fillcolor=\"#7fc97f\"];");
            foreach (var state in intermediateStates)
            {
                code.AppendLine($"        {state.Id.ToDotId()} [label = \"{state.Name}\"];");
            }

            code.AppendLine($"");

            foreach (var state in states)
            {
                foreach (var transition in state.Transitions)
                {
                    code.AppendLine($"        {state.Id.ToDotId()} -> {transition.ToState.Id.ToDotId()} [ label = \"{transition.Event.Name}\"];");
                }
            }
            code.AppendLine("}");

            return code.ToString();
        }

        private static string ToDotId(this Guid id)
        {
            return String.Concat(id.ToString("N").Select(c => (char)(c + 17)));
        }
    }
}
