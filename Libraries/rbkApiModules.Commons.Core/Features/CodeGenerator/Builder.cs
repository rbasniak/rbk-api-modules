using Microsoft.AspNetCore.Builder;

namespace rbkApiModules.Commons.Core.CodeGeneration;

public static class Builder
{ 
    public static IApplicationBuilder UseRbkCodeGenerationModule(this IApplicationBuilder app, Action<CodeGenerationModuleOptions> configureOptions)
    {
        configureOptions(CodeGenerationModuleOptions.Instance);

        return app;
    }
}

public class CodeGenerationModuleOptions
{
    internal Dictionary<string, List<string>> IgnoreOptions = new Dictionary<string, List<string>>();
    private static CodeGenerationModuleOptions _instance;
    private string _currentScope;

    internal static CodeGenerationModuleOptions Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new CodeGenerationModuleOptions();
            }

            return _instance;
        }
    }

    public CodeGenerationModuleOptions ForScope(string scope)
    {
        if (IgnoreOptions.TryGetValue(scope, out _))
        {
            throw new ApplicationException($"The scope '{scope}' was already setup");
        }

        _currentScope = scope;

        IgnoreOptions.Add(scope, new List<string>());

        return this;
    }

    public CodeGenerationModuleOptions IgnoreRoutesContaining(string pattern)
    {
        if (_currentScope == null)
        {
            throw new ApplicationException($"You need to define a scope. Please use the '{nameof(ForScope)}' method first.");
        }

        IgnoreOptions.TryGetValue(_currentScope, out var list);

        list.Add(pattern);

        return this;
    }
}
