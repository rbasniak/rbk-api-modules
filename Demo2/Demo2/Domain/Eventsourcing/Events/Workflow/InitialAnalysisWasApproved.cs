namespace Demo2.Domain.Events;

public class ChangeRequestInitialAnalysisWasApproved
{
    public class V1 : DomainEvent
    {
        public V1(string username, Guid aggregateId) : base(username, aggregateId)
        {

        }

        public override EventSummary Summary => new EventSummary
        {
            Description = "Usuário geral enviou a solicitação para aprovação"
        };
    }
}