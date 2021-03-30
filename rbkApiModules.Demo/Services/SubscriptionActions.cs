using Microsoft.EntityFrameworkCore;
using rbkApiModules.Payment.SqlServer;
using rbkApiModules.Paypal.SqlServer;
using System;
using System.Threading.Tasks;

namespace rbkApiModules.Demo.Services
{
    public class SubscriptionActions : ISubscriptionActions
    {
        private readonly DbContext _context;

        public SubscriptionActions(DbContext context)
        {
            _context = context;
        }

        public void OnCancelationSuccess(Guid clientId, string planName)
        {
        }

        public void OnCreateSuccess(Guid clientId, string planName)
        {
        }
    }
}
