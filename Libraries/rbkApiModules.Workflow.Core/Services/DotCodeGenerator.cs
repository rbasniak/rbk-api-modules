using System.Text;

namespace rbkApiModules.Workflow.Core;

public static class DotCodeGenerator
{
    public static string GenerateCode(State[] states)
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
