using Demo2.Relational;
using rbkApiModules.Commons.Core;
using System;
using System.ComponentModel.DataAnnotations;

namespace Demo2.EventSourcing;

public class EvidenceAttachment : BaseEntity
{
    public EvidenceAttachment()
    {

    }
    public EvidenceAttachment(string name, Guid typeId, long size, string path, string filename, string commentary)
    {
        Name = name;
        TypeId = typeId;
        Size = size;
        Path = path;
        Filename = filename;
        Commentary = commentary;
        AdditionDate = DateTime.UtcNow;
    }

    public virtual string Name { get; internal set; }

    public virtual Guid TypeId { get; internal set; }
    public virtual AttachmentType Type { get; internal set; }

    public virtual long Size { get; internal set; }

    public virtual DateTime AdditionDate { get; internal set; }

    public virtual string Path { get; internal set; }

    public virtual string Filename { get; internal set; }

    public virtual string Commentary { get; internal set; }
}
