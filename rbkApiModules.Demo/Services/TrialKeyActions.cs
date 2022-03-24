using Microsoft.EntityFrameworkCore;
using rbkApiModules.Payment.SqlServer;
using System;

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
