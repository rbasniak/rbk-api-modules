namespace Demo2.Domain.Events;

public class ChangeRequestInitialAnalysisWasRepproved
{
    public class V1 : DomainEvent
    {
        public V1(Guid changeRequestId, string reason) : base(changeRequestId)
        {
            Reason = reason;
        }

        public string Reason { get; protected set; }

        public override string Description => "Admin reprovou a solicitação durante a análise inicial";
    }
}