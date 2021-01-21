using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace rbkApiModules.Workflow
{
    public interface IStatesCacheService
    {
        bool IsInitialized { get; }
    }

    public interface IEventsCacheService
    {
        bool IsInitialized { get; }
    }

    public abstract class BaseCacheService<T> where T: DbContext
    {
        public bool IsInitialized { get; }

        protected BaseCacheService(IServiceScopeFactory scopeProvider)
        {
            try
            {
                using (var scope = scopeProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<T>();

                    Initialise(context);

                    IsInitialized = true;
                }
            }
            catch
            {
                throw new ApplicationException("Não foi possível inicializar o serviço de eventos");
            }
        }

        public abstract void Initialise(T context);
    }
}
