namespace Demo2.Domain.Events;

public partial class ChangeRequestEvents 
{
    public class GeneralUserSentToApproval
    {
        public class V1 : DomainEvent
        {
            public V1(Guid changeRequestId) : base(changeRequestId)
            {

            }

            public override string EventDescription => "Usuário geral enviou a solicitação para aprovação";
        }
    }
}