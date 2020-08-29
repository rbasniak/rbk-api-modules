using System;

namespace rbkApiModules.UIAnnotations
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class DialogDataAttribute : Attribute
    {
        public DialogDataAttribute(OperationType operation, string name)
        {
            Operation = operation;
            Name = name;
        }

        public string Name { get; }
        public OperationType Operation { get; }
        public DataSource Source { get; set; }
        public object DefaultValue { get; set; }
        public string DependsOn { get; set; }
        public DialogControlTypes ForcedType { get; set; }
        public string Group { get; set; }
        public string Mask { get; set; }
        public bool Nullable { get; set; }
    }

    public enum OperationType
    {
        Create,
        Update,
        CreateAndUpdate
    }
}
