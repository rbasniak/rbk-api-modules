using rbkApiModules.Commons.Core;

namespace Demo2.Samples.Relational.Domain.Models;

public class ChangeRequestType : BaseEntity
{
    public ChangeRequestType()
    {

    }
    public ChangeRequestType(string name)
    {
        Name = name;
    }

    public virtual string Name { get; set; }
}
