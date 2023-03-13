using rbkApiModules.Commons.Core;
using System;
using System.ComponentModel.DataAnnotations;

namespace Demo2.Relational;

public class Attachment: BaseEntity
{
    public Attachment()
    {
    }

    public Attachment(string name, AttachmentType type, long size, string path, string filename, ChangeRequest changeRequest)
    {
        Name = name;
        Type = type;
        Size = size;
        Path = path;
        Filename = filename;
        ChangeRequest = changeRequest;
    }


    public virtual string Name { get; set; }

    public virtual Guid TypeId { get; set; }
    public virtual AttachmentType Type { get; set; }

    public virtual Guid ChangeRequestId { get; set; }
    public virtual ChangeRequest ChangeRequest { get; set; }

    public virtual long Size { get; set; }

    public virtual string Path { get; set; }

    public virtual string Filename { get; set; }
}
