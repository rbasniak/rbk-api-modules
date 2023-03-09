using Demo2.EventSourcing;
using rbkApiModules.Commons.Core;
using System;
using System.ComponentModel.DataAnnotations;

namespace Demo2.EventSourcing;

public class Attachment : BaseEntity
{
    public Attachment()
    {
    }

    public Attachment(string name, AttachmentType type, long size, string path, string filename)
    {
        Name = name;
        Type = type;
        Size = size;
        Path = path;
        Filename = filename;
    }


    public virtual string Name { get; set; }

    public virtual AttachmentType Type { get; set; }

    public virtual long Size { get; set; }

    public virtual string Path { get; set; }

    public virtual string Filename { get; set; }
}
