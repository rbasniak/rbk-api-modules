namespace rbkApiModules.Commons.Core.Features.ApplicationOptions;

public interface IApplicationOptionsManager
{
    TOptions GetOptions<TOptions>(string? tenantId = null, string? username = null)
        where TOptions : class, IApplicationOptions, new();
}

