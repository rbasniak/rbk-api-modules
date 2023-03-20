using rbkApiModules.Commons.Core;

namespace Demo2.Samples.Relational.Domain.Models;

public class ChangeRequestSource : BaseEntity
{
    public ChangeRequestSource()
    {

    }
    public ChangeRequestSource(string name)
    {
        Name = name;
    }

    public virtual string Name { get; set; }

}
