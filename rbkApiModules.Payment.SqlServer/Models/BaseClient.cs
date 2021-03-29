using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rbkApiModules.Payment.SqlServer
{
    public abstract class BaseClient : BaseEntity
    {
        protected readonly HashSet<Subscription> _subscriptions;

        protected BaseClient() { }

        public BaseClient(Plan plan)
        {
            _subscriptions = new HashSet<Subscription>();

            Plan = plan;
        }

        public bool SubscriptionInCancelation { get; private set; }
        public bool SubscriptionHasExpired { get; private set; }

        public Guid? PlanId { get; private set; }
        public Plan Plan { get; private set; }

        public Guid? TrialKeyId { get; private set; }
        public TrialKey TrialKey { get; private set; }

        public IEnumerable<Subscription> Subscriptions => _subscriptions?.ToList();

        public void ChangePlan(Plan plan)
        {
            Plan = plan;
        }

        public void RemoveTrial()
        {
            TrialKey = null;
        }

        public void SetTrialKey(TrialKey trialKey)
        {
            TrialKey = trialKey;
        }

        public void SetSubscriptionInCancelation(bool value)
        {
            SubscriptionInCancelation = value;
        }

        public void SetSubscriptionHasExpired(bool value)
        {
            SubscriptionHasExpired = value;
        }

        public void UseTrialPlan()
        {
            Plan = TrialKey.Plan;
        }
    }
}