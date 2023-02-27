using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

public class DocumentAddedToChangeRequest
{
    public class V1 : DomainEvent
    {
        protected V1()
        {

        }

        public V1(Guid changeRequestId, string name, string number, string source) : base(changeRequestId)
        {
            DocumentId = Guid.NewGuid();
            Name = name;
            Number = number;
            Source = source;
        }

        public Guid DocumentId { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string Source { get; set; }

        public override string Description => "Usuário adicionou um novo documento";
    }
}