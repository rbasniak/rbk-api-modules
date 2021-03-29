using rbkApiModules.Infrastructure.Models;
using System.Collections.Generic;
using System.Linq;

namespace rbkApiModules.Payment.SqlServer
{
    public class Plan : BaseEntity
    {
        private readonly HashSet<Subscription> _subscriptions;
        private readonly HashSet<BaseClient> _clients;

        protected Plan() { }

        public Plan(string name,
            int duration,
            double price,
            bool isActive,
            bool isDefault, 
            string paypalId,
            string paypalSandboxId)
        {
            _subscriptions = new HashSet<Subscription>();
            _clients = new HashSet<BaseClient>();

            Update(name,
                duration,
                price,
                isActive,
                isDefault,
                paypalId,
                paypalSandboxId);
        }

        public string Name { get; private set; }
        public bool IsActive { get; private set; }
        public int Duration { get; private set; }
        public double Price { get; private set; }
        public bool IsDefault { get; private set; }
        public string PaypalId { get; private set; }
        public string PaypalSandboxId { get; private set; } 

        public IEnumerable<Subscription> Subscriptions => _subscriptions?.ToList();
        public IEnumerable<BaseClient> Clients => _clients?.ToList();

        public void Update(string name,
            int duration,
            double price,
            bool isActive,
            bool isDefault,
            string paypalId,
            string paypalSandboxId)
        {
            Name = name;
            Duration = duration;
            Price = price;
            IsActive = isActive;
            IsDefault = isDefault;
            PaypalId = paypalId;
            PaypalSandboxId = paypalSandboxId;
        }
    }
}
