namespace rbkApiModules.Commons.Core.CodeGeneration;

public class TypeInfo
{
    public TypeInfo(Type type)
    {
        if (type.IsList() && type != typeof(Dictionary<string, string>))
        {
            IsList = true;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                Type = type.GenericTypeArguments.First();
            }
            else
            {
                Type = type.GetInterfaces().Single(x => x.Name == typeof(IEnumerable<>).Name).GenericTypeArguments.First();

            }
        }
        else
        {
            Type = type;
        }

        if (Type.Name == typeof(Nullable<>).Name)
        {
            Nullable = true;
            Type = Type.GenericTypeArguments.First();
        }

        if (Type.Name == typeof(SimpleNamedEntity<>).Name && Type.GenericTypeArguments.First() == typeof(Int32))
        {
            Name = "{ id: number, name: string }";
        }
        if (Type.Name == typeof(SimpleNamedEntity<>).Name && Type.GenericTypeArguments.First() != typeof(Int32))
        {
            Name = nameof(SimpleNamedEntity);
        }
        else if (Type != typeof(Object) && (Type.IsAssignableFrom(typeof(TreeNode))
            || Type.BaseType == typeof(TreeNode)
            || (Type.BaseType.IsGenericType && Type.BaseType.GetGenericTypeDefinition() == typeof(TreeNode<>))))
        {
            Name = "TreeNode";
        }
        else if (Type == typeof(Dictionary<string, string>))
        {
            Name = "{ [key: string]: string }";
        }
        else
        {
            Name = Type.FullName.Split('.').Last().Replace("[]", "").Replace("+Request", "").Replace("+", "");
        }
    }

    public Type Type { get; set; }
    public bool IsList { get; set; }
    public bool Nullable { get; set; }
    public string Name { get; set; }
    public List<PropertyInfo> Properties { get; set; }

    public override string ToString()
    {
        return Name + (IsList ? "[]" : "");
    }
}