using rbkApiModules.Commons.Core;

namespace Demo2.Samples.Relational.Domain.Models;

public class FicCategory : BaseEntity
{
    public FicCategory()
    {

    }
    public FicCategory(string name)
    {
        Name = name;
    }

    public virtual string Name { get; set; }
}
