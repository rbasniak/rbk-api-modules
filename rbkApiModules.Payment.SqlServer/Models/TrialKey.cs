using rbkApiModules.Infrastructure.Models;
using System;

namespace rbkApiModules.Payment.SqlServer
{
    public class TrialKey : BaseEntity
    {
        protected TrialKey() { }

        public TrialKey(Plan plan, int numberOfDays)
        {
            Plan = plan;
            TrialPeriod = numberOfDays;
            CreatedOn = DateTime.UtcNow;
        }

        public DateTime CreatedOn { get; private set; }
        public DateTime? UsedOn { get; private set; }
        public int TrialPeriod { get; private set; }
        public DateTime? ExpiresOn => CreatedOn.AddDays(TrialPeriod);
        public bool IsAvailable => UsedOn == null;
        public Guid PlanId { get; private set; }
        public Plan Plan { get; private set; }

        public void Activate()
        {
            UsedOn = DateTime.UtcNow;
        }
    }
}