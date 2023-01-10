namespace rbkApiModules.Commons.Core.CodeGeneration;

public class PropertyInfo
{
    public PropertyInfo(System.Reflection.PropertyInfo propertyInfo)
    {
        Name = propertyInfo.Name;
        Type = new TypeInfo(propertyInfo.PropertyType);
    }

    public PropertyInfo(string name, Type type)
    {
        Name = name;
        Type = new TypeInfo(type);
    }

    public string Name { get; set; }
    public TypeInfo Type { get; set; }
}


