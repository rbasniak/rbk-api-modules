using System;

namespace  rbkApiModules.Payment.SqlServer
{
    public interface ISubscriptionActions
    {
        public void OnCreateSuccess(Guid clientId, string planName);
        public void OnCancelationSuccess(Guid clientId, string planName);
    }
}
