using MediatR;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Notifications.Core;

public class UpdateNotificationStatus
{
    public class Request : AuthenticatedRequest, IRequest<CommandResponse>
    {
        public Guid[] NotificationIds { get; set; }

        public NotificationStatus Status { get; set; }
    }

    public class Handler : IRequestHandler<Request, CommandResponse>
    {
        private readonly INotificationsService _notificationsService;

        public Handler(INotificationsService notificationsService)
        {
            _notificationsService = notificationsService;
        }

        public async Task<CommandResponse> Handle(Request request, CancellationToken cancellation)
        {
            await _notificationsService.UpdateStatusAsync(request.Identity.Username, request.NotificationIds, request.Status, cancellation);

            return CommandResponse.Success();
        }
    }
}
