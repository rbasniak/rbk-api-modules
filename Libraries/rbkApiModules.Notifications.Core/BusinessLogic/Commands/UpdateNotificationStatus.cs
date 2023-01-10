using MediatR;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Notifications.Core;

public class UpdateNotificationStatus
{
    public class Command : AuthenticatedCommand, IRequest<CommandResponse>
    {
        public Guid[] NotificationIds { get; set; }

        public NotificationStatus Status { get; set; }
    }

    public class Handler : IRequestHandler<Command, CommandResponse>
    {
        private readonly INotificationsService _notificationsService;

        public Handler(INotificationsService notificationsService)
        {
            _notificationsService = notificationsService;
        }

        public async Task<CommandResponse> Handle(Command request, CancellationToken cancellation)
        {
            await _notificationsService.UpdateStatusAsync(request.Identity.Username, request.NotificationIds, request.Status, cancellation);

            // return (null, notifications.Select(x => x.Id));

            throw new NotImplementedException("Pq esta retornando os ids?");
            return CommandResponse.Success();
        }
    }
}
