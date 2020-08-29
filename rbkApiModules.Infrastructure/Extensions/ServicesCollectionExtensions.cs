using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace rbkApiModules.Infrastructure
{
    public static class ServicesCollectionExtensions
    {
        /// <summary>
        /// Método para registrar automaticamente serviços da aplicação no container
        /// de injeção de dependências, para isso basta nomear a interface com o nome
        /// da classe prefixado com I, exemplo: DomainService e IDomainService
        /// </summary>
        /// <param name="assemblies">Lista de assemblies que devem ser escaneados para
        /// buscar as interfaces para registrar</param>
        public static void RegisterApplicationServices(this IServiceCollection services, params Assembly[] assemblies)
        {
            //var assemblyPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), assemblyName + ".dll");
            //var assembly = Assembly.Load(AssemblyLoadContext.GetAssemblyName(assemblyPath));
            //var assembly = Assembly.GetAssembly()

            var isFirstGlobally = true;

            foreach (var assembly in assemblies)
            {
                var isFirstOnAssembly = true;

                var classTypes = assembly.ExportedTypes.Select(t => t.GetTypeInfo()).Where(t => t.IsClass && !t.IsAbstract);

                foreach (var type in classTypes.Where(x => !x.IsNested))
                {
                    var interfaces = type.ImplementedInterfaces.Select(i => i.GetTypeInfo());

                    // foreach (var handlerType in interfaces.Where(i => i.IsGenericType))
                    foreach (var handlerType in interfaces)
                    {
                        if (handlerType.Name == $"I{type.Name}")
                        {
                            if (isFirstGlobally)
                            {
                                Debug.WriteLine($"\nRegistering application services");
                            }

                            if (isFirstOnAssembly)
                            {
                                Debug.WriteLine($"  -> {assembly.GetName().Name}");
                            }

                            Debug.WriteLine($"    -> {type.Name}");
                            services.AddScoped(handlerType.AsType(), type.AsType());

                            isFirstGlobally = false;
                            isFirstOnAssembly = false;
                        }
                    }
                }
            }
        }
    }
}
