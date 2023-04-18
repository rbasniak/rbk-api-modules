using MediatR;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Notifications.Core;

public class DeleteNotification
{
    public class Request : AuthenticatedRequest, IRequest<CommandResponse>
    {
        public Guid[] NotificationIds { get; set; }
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
            await _notificationsService.DeleteAsync(request.Identity.Username, request.NotificationIds, cancellation);

            return CommandResponse.Success();
        }
    }
}
