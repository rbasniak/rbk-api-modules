using rbkApiModules.Commons.Core;

namespace Demo2.Samples.Relational.Domain.Models;

public class DocumentCategory : BaseEntity
{
    public DocumentCategory()
    {

    }
    public DocumentCategory(string name)
    {
        Name = name;
    }

    public virtual string Name { get; set; }
}
