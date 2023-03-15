using rbkApiModules.Commons.Core;
using System.Collections.Generic;
using System.Linq;

namespace Demo2.Relational;

public class DocumentCategory: BaseEntity
{
    public DocumentCategory()
    {

    }
    public DocumentCategory(string name)
    {
        Name = name;
    }

    public virtual string Name  { get; set; }
}
