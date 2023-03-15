using rbkApiModules.Commons.Core;
using System.Collections.Generic;
using System.Linq;

namespace Demo2.Relational;

public class FicCategory: BaseEntity
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
