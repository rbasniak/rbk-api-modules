using System;

namespace  rbkApiModules.Payment.SqlServer
{
    public interface ITrialKeyActions
    {
        public void OnCreateSuccess(Guid clientId, Guid trialKeyId, string days, string planName);
    }
}
