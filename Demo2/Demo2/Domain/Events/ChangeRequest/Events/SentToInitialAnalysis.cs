namespace Demo2.Domain.Events;

public partial class ChangeRequestEvents 
{
    public class SentToInitialAnalysis
    {
        public class V1 : DomainEvent
        {
            public V1(Guid changeRequestId) : base(changeRequestId)
            {

            }

            public override string Description => "Usuário geral enviou a solicitação para aprovação";
        }
    }
}