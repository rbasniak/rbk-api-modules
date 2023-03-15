using Demo2.Domain.Events.Infrastructure;
using Demo2.EventSourcing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

public class AttachmentAddedToChangeRequest
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1(string username, Guid changeRequestId, string name, string filename, string path, Guid typeId, int size) : base(username, changeRequestId)
        {
            Name = name;
            Filename = filename;
            Path = path;
            TypeId = typeId;
            Size = size;
        }

        public string Name { get; protected set; }
        public string Filename { get; protected set; }
        public string Path { get; protected set; }
        public Guid TypeId { get; protected set; }
        public int Size { get; protected set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.AddAttachment(Name, TypeId, Size, Path, Filename);
        }
    }
}