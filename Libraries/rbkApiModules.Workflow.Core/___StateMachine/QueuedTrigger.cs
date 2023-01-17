namespace Stateless
{
    internal class QueuedTrigger<TTrigger>
    {
        public TTrigger Trigger { get; set; }
        public object[] Args { get; set; }
    }
}
