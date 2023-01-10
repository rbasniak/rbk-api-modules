using Stateless.Reflection;

namespace Stateless.Graph
{
    /// <summary>
    /// Class to generate a DOT graph in UML format
    /// </summary>
    public static class UmlDotGraph<TState, TTrigger>
    {
        /// <summary>
        /// Generate a UML DOT graph from the state machine info
        /// </summary>
        /// <param name="machineInfo"></param>
        /// <returns></returns>
        public static string Format(StateMachineInfo<TState, TTrigger> machineInfo)
        {
            var graph = new StateGraph<TState, TTrigger>(machineInfo);

            return graph.ToGraph(new UmlDotGraphStyle<TState, TTrigger>());
        }

    }
}
