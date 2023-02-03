namespace Demo2.Domain.Events;

public partial class ChangeRequestEvents 
{
    public class AdminRequestedMoreInfoDuringInitialAnalysis
    {
        public class V1 : DomainEvent
        {
            public V1(Guid changeRequestId, string informationNeeded) : base(changeRequestId)
            {
                InformationNeeded = informationNeeded;
            }

            public string InformationNeeded { get; protected set; }

            public override string EventDescription => "Admin solicitou mais informações durante a análise inicial";
        }
    }
}