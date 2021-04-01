using Microsoft.EntityFrameworkCore;
using rbkApiModules.Payment.SqlServer;
using rbkApiModules.Paypal.SqlServer;
using System;
using System.Threading.Tasks;

namespace rbkApiModules.Demo.Services
{
    public class TrialKeyActions : ITrialKeyActions
    {
        private readonly DbContext _context;

        public TrialKeyActions(DbContext context)
        {
            _context = context;
        }

        public void OnCreateSuccess(Guid clientId, Guid trialKeyId, string days, string planName)
        {
        }
    }
}
