namespace rbkApiModules.Commons.Core.CodeGeneration;

public class TypescriptMethod
{
    public TypescriptMethod()
    {

    }

    public string Name { get; set; }
    public TypescriptProperty ReturnType { get; set; }
    public List<TypescriptProperty> Parameters { get; set; }
}
