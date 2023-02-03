namespace Demo2.Domain.Events;

public partial class ChangeRequestEvents 
{
    public class AdminPickedForInitialAnalysis
    {
        public class V1 : DomainEvent
        {
            public V1(Guid changeRequestId, string adminUsername) : base(changeRequestId)
            {
                AdminUsername = adminUsername;
            }

            public string AdminUsername { get; protected set; }

            public override string EventDescription => "Admin pegou a solicitação para análise inicial";
        }
    }
}