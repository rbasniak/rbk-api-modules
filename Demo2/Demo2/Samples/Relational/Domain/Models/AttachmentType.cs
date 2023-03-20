using rbkApiModules.Commons.Core;

namespace Demo2.Samples.Relational.Domain.Models;

public class AttachmentType : BaseEntity
{
    public AttachmentType()
    {

    }
    public AttachmentType(string name, string extension)
    {
        Name = name;
        Extension = extension;
    }

    public virtual string Name { get; set; }

    public virtual string Extension { get; set; }

}
