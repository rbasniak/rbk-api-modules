using System;

namespace rbkApiModules.Workflow
{
    public class ClaimToEvent
    {
        protected ClaimToEvent()
        {

        }

        public ClaimToEvent(Event @event, string claim)
        {
            Claim = claim;
            Event = @event;
        }

        public virtual string Claim { get; private set; }

        public virtual Guid EventId { get; private set; }
        public virtual Event Event { get; private set; }

        public override string ToString()
        {
            return Claim +  " - " + Event?.Name;
        }
    }
}
