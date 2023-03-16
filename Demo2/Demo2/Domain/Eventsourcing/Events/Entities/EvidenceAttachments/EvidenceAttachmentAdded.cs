using Demo2.Domain.Events.Infrastructure;
using Demo2.EventSourcing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

public class EvidenceAttachmentAddedToChangeRequest
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1()
        {

        }
        public V1(string username, Guid aggregateId, string name, string filename, string path, Guid typeId, int size, string comments) : base(username, aggregateId)
        {
            Name = name;
            Filename = filename;
            Path = path;
            TypeId = typeId;
            Size = size;
            Comments = comments;
        }

        public string Name { get; set; }
        public string Filename { get; set; }
        public string Path { get; set; }
        public Guid TypeId { get; set; }
        public int Size { get; set; }
        public string Comments { get; set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.AddEvidenceAttachment(Name, TypeId, Size, Path, Filename, Comments);
        }
    }
}