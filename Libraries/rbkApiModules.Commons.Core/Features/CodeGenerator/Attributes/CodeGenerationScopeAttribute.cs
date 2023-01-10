namespace rbkApiModules.Commons.Core.CodeGeneration;

[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
public class CodeGenerationScopeAttribute : Attribute
{
    public CodeGenerationScopeAttribute(params string[] scopes)
    {
        if (scopes != null)
        {
            Scopes = scopes;
        }
        else
        {
            Scopes = new string[0];
        }
    }

    public string[] Scopes { get; set; }
}
