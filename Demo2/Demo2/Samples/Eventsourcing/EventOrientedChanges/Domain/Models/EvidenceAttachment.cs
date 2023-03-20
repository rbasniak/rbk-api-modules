using Demo2.Samples.Relational.Domain.Models;
using rbkApiModules.Commons.Core;

namespace Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Models;

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
