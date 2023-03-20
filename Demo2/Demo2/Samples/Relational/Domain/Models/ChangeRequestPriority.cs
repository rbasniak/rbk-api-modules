using rbkApiModules.Commons.Core;

namespace Demo2.Samples.Relational.Domain.Models;

public class ChangeRequestPriority : BaseEntity
{
    public ChangeRequestPriority()
    {

    }
    public ChangeRequestPriority(string name, string color)
    {
        Name = name;
        Color = color;
    }

    public virtual string Name { get; set; }

    public virtual int Order { get; set; }

    public virtual string Color { get; set; }

}
