using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

public class EvidenceAttachmentAddedToChangeRequest
{
    public class V1 : DomainEvent
    {
        public V1(string username, Guid changeRequestId) : base(username, changeRequestId)
        {

        }

        public override EventSummary Summary => new EventSummary
        {
            Description = "Usuário geral enviou a solicitação para aprovação"
        };
    }
}