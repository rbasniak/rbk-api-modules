using Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Models;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Infrastructure.Models;

namespace Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Events.Entities.Attachments;

public class AttachmentAddedToChangeRequest
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1()
        {

        }
        public V1(string username, Guid aggregateId, string name, string filename, string path, Guid typeId, int size) : base(username, aggregateId)
        {
            Name = name;
            Filename = filename;
            Path = path;
            TypeId = typeId;
            Size = size;
        }

        public string Name { get; set; }
        public string Filename { get; set; }
        public string Path { get; set; }
        public Guid TypeId { get; set; }
        public int Size { get; set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.AddAttachment(Name, TypeId, Size, Path, Filename);
        }
    }
}