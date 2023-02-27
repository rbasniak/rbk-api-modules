using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Comments.Core;

public static class CoreCommentBuilder
{
    public static IServiceCollection AddRbkCommentsCore(this IServiceCollection services, RbkCommentsOptions options)
    {
        services.RegisterApplicationServices(Assembly.GetAssembly(typeof(ICommentsService)));

        if (options._userDataServiceType == null)
        {
            throw new InvalidOperationException($"The implementation for {nameof(IUserdataCommentService)} was not provided");
        }
        
        services.AddScoped(typeof(IUserdataCommentService), options._userDataServiceType);

        AssemblyScanner
            .FindValidatorsInAssembly(Assembly.GetAssembly(typeof(CommentEntity.Request)))
                .ForEach(result => services.AddScoped(result.InterfaceType, result.ValidatorType));


        return services;
    }
}

public class RbkCommentsOptions
{
    internal Type _userDataServiceType;

    public RbkCommentsOptions SetCommentsUserdataService(Type type)
    {
        if (!type.GetInterfaces().Any(x => x == typeof(IUserdataCommentService)))
        {
            throw new ArgumentException($"The provided service does not implement the {nameof(IUserdataCommentService)} interface");
        }

        _userDataServiceType = type;

        return this;
    }
 }
