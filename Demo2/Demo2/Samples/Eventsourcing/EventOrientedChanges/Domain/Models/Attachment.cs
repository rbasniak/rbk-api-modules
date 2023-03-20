using rbkApiModules.Commons.Core;

namespace Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Models;

public class Attachment : BaseEntity
{
    public Attachment()
    {
    }

    public Attachment(string name, Guid typeId, long size, string path, string filename)
    {
        Name = name;
        TypeId = typeId;
        Size = size;
        Path = path;
        Filename = filename;
    }


    public virtual string Name { get; internal set; }

    public virtual Guid TypeId { get; internal set; }

    public virtual long Size { get; internal set; }

    public virtual string Path { get; internal set; }

    public virtual string Filename { get; internal set; }
}
