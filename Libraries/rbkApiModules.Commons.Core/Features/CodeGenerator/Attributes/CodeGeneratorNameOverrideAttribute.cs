namespace rbkApiModules.Commons.Core.CodeGeneration;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class CodeGeneratorNameOverrideAttribute : Attribute
{
    public CodeGeneratorNameOverrideAttribute(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        Name = name;
    } 

    public string Name{ get; }
}