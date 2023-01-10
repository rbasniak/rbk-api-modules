namespace rbkApiModules.Commons.Core.CodeGeneration;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class NgxsStoreBehaviorAttribute : Attribute
{
    public NgxsStoreBehaviorAttribute()
    {
        Behavior = StoreBehavior.General;
    }

    public NgxsStoreBehaviorAttribute(StoreBehavior behavior)
    {
        Behavior = behavior;
    }

    public StoreBehavior Behavior { get; set; }
}

public enum StoreBehavior
{
    // Most common type of store, with a basic CRUD
    General,

    // Store in which all the data of the store is returned in every CRUD
    // endpoint and the store contents is fully replaced every time
    ReplaceAll,
}