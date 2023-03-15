using rbkApiModules.Commons.Core;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Demo2.Relational;

public class AttachmentType: BaseEntity
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
