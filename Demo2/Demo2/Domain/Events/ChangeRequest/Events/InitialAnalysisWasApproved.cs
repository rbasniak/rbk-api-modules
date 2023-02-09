namespace Demo2.Domain.Events;

public partial class ChangeRequestEvents 
{
    public class InitialAnalysisWasApproved
    {
        public class V1 : DomainEvent
        {
            public V1(Guid changeRequestId) : base(changeRequestId)
            {

            }

            public override string Description => "Admin aprovou a solicitação durante a análise inicial";
        }
    }
}