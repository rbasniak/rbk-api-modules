namespace rbkApiModules.Commons.Core.CodeGeneration;

public class TypescriptType
{
    public TypescriptType(TypeInfo type)
    {
        IsNative = false;
        Name = type.Name;

        if (type.Type.FullName == typeof(String).FullName)
        {
            IsNative = true;
            Name = "string";
        }

        if (type.Type.FullName == typeof(Guid).FullName)
        {
            IsNative = true;
            Name = "string";
        }

        if (type.Type.FullName == typeof(Boolean).FullName)
        {
            IsNative = true;
            Name = "boolean";
        }

        if (type.Type.FullName == typeof(DateTime).FullName)
        {
            IsNative = true;
            Name = "Date";
        }

        if (type.Type.FullName == typeof(Single).FullName)
        {
            IsNative = true;
            Name = "number";
        }


        if (type.Type.FullName == typeof(Double).FullName)
        {
            IsNative = true;
            Name = "number";
        }

        if (type.Type.FullName == typeof(Decimal).FullName)
        {
            IsNative = true;
            Name = "number";
        }

        if (type.Type.FullName == typeof(Int16).FullName)
        {
            IsNative = true;
            Name = "number";
        }

        if (type.Type.FullName == typeof(Int32).FullName)
        {
            IsNative = true;
            Name = "number";
        }

        if (type.Type.FullName == typeof(Int64).FullName)
        {
            IsNative = true;
            Name = "number";
        }

        if (type.Type.FullName == typeof(Object).FullName)
        {
            IsNative = true;
            Name = "any";
        }

        if (type.Type.FullName == typeof(SimpleNamedEntity<int>).FullName)
        {
            IsNative = true;
            Name = "{ id: number, name: string }";
        }
    }

    public string Name { get; set; }
    public bool IsNative { get; set; }

    public override string ToString()
    {
        return Name;
    }
}
