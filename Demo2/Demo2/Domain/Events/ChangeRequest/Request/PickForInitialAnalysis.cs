namespace Demo2.Domain.Events;

// https://itnext.io/implementing-event-store-in-c-8a27138cc90

public partial class ChangeRequestCommands
{
    public class PickForInitialAnalysis
    {
        public class Request
        {
            public Request()
            {
                Id = Guid.NewGuid();
            }

            public Guid Id { get; protected set; }
            public string RequestedBy { get; protected set; }
            public string CreatedBy { get; protected set; }
            public string Description { get; protected set; }
            public string Title { get; protected set; }
        }
    }
}