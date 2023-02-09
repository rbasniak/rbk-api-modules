using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

public partial class ChangeRequestEvents
{
    public class CreatedByGeneralUser
    {
        public class V1 : DomainEvent
        {
            public V1(Guid changeRequestId, string requestedBy, string createdBy, string description, string title) : base(changeRequestId)
            {
                ChangeRequestId = changeRequestId;
                RequestedBy = requestedBy;
                CreatedBy = createdBy;
                Description2 = description;
                Title = title;
            }

            public Guid ChangeRequestId { get; set; }
            public string RequestedBy { get; set; }
            public string CreatedBy { get; set; }
            public string Description2 { get; set; }
            public string Title { get; set; }

            public override string Description => "Usuário geral criou uma nova solicitação";
        }
    }
}