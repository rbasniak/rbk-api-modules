using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using rbkApiModules.Utilities.Extensions;
using System.Reflection;

namespace rbkApiModules.Payment.SqlServer
{
    public static class Builder
    {
        public static void AddRbkApiPaymentModule<TSubscription, TTrialKey>(this IServiceCollection services) 
            where TSubscription : class, ISubscriptionActions 
            where TTrialKey : class, ITrialKeyActions
        {
            services.RegisterApplicationServices(Assembly.GetAssembly(typeof(TSubscription)));
            services.RegisterApplicationServices(Assembly.GetAssembly(typeof(TTrialKey)));

            services.AddTransient<ISubscriptionActions, TSubscription>();
            services.AddTransient<ITrialKeyActions, TTrialKey>();

            AssemblyScanner
                .FindValidatorsInAssembly(Assembly.GetAssembly(typeof(CreatePlan.Command)))
                    .ForEach(result => services.AddScoped(result.InterfaceType, result.ValidatorType));
        }
    }
}
