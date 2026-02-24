// TODO: DONE, REVIEWED

namespace rbkApiModules.Commons.Core;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class EventNameAttribute : Attribute
{
    public EventNameAttribute(string name, short version = 1)
    {
        ArgumentNullException.ThrowIfNull(name);
        
        if (version < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(version), "Version must be greater than or equal to 1");
        }

        Name = name;
        Version = version;
    }

    public string Name { get; }

    public short Version { get; }
} 