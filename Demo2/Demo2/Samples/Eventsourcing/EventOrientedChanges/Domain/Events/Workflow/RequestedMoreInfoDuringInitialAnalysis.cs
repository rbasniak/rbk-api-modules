using Demo2.Samples.Eventsourcing.EventOrientedChanges.Infrastructure.Models;

namespace Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Events.Workflow;

public class RequestedMoreInfoDuringChangeRequestInitialAnalysis
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