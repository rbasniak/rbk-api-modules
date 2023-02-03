namespace Demo2.Domain.Events;

public partial class ChangeRequestEvents
{
    public class GeneralUserCreated
    {
        public class V1 : DomainEvent<ChangeRequestCommands.CreateByGeneralUser.Request>
        {
            public V1(ChangeRequestCommands.CreateByGeneralUser.Request payload) : base(payload.Id, payload)
            {
            }

            public override string Description => "Usuário geral criou uma nova solicitação";
        }
    }
}