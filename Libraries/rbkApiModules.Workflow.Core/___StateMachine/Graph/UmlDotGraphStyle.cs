﻿using System.Text;

namespace Stateless.Graph
{
    /// <summary>
    /// Generate DOT graphs in basic UML style
    /// </summary>
    public class UmlDotGraphStyle<TState, TTrigger> : GraphStyleBase<TState, TTrigger>
    {
        /// <summary>Get the text that starts a new graph</summary>
        /// <returns></returns>
        public override string GetPrefix()
        {
            return "digraph {\n"
                      + "compound=true;\n"
                      + "node [shape=Mrecord]\n"
                      + "rankdir=\"LR\"\n";
        } 

        /// <summary>
        /// Generate the text for a single state
        /// </summary>
        /// <param name="state">The state to generate text for</param>
        /// <returns></returns>
        public override string FormatOneState(State<TState, TTrigger> state)
        {
            if ((state.EntryActions.Count == 0) && (state.ExitActions.Count == 0))
                return $"\"{state.StateName}\" [label=\"{state.StateName}\"];\n";

            string f = $"\"{state.StateName}\" [label=\"{state.StateName}|";

            List<string> es = new List<string>();
            es.AddRange(state.EntryActions.Select(act => "entry / " + act));
            es.AddRange(state.ExitActions.Select(act => "exit / " + act));

            f += String.Join("\\n", es);

            f += "\"];\n";

            return f;
        }

        /// <summary>
        /// Generate text for a single transition
        /// </summary>
        /// <param name="sourceNodeName"></param>
        /// <param name="trigger"></param>
        /// <param name="actions"></param>
        /// <param name="destinationNodeName"></param>
        /// <param name="guards"></param>
        /// <returns></returns>
        public override string FormatOneTransition(string sourceNodeName, string trigger, IEnumerable<string> actions, string destinationNodeName, IEnumerable<string> guards)
        {
            string label = trigger ?? "";

            if (actions?.Count() > 0)
                label += " / " + string.Join(", ", actions);

            if (guards.Any())
            {
                foreach (var info in guards)
                {
                    if (label.Length > 0)
                        label += " ";
                    label += "[" + info + "]";
                }
            }

            return FormatOneLine(sourceNodeName, destinationNodeName, label);
        }

        internal string FormatOneLine(string fromNodeName, string toNodeName, string label)
        {
            return $"\"{fromNodeName}\" -> \"{toNodeName}\" [style=\"solid\", label=\"{label}\"];";
        }
    }
}
