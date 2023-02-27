using MediatR;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Notifications.Core;

public class GetNotifications
{
    public class Request : AuthenticatedRequest, IRequest<QueryResponse>
    {
        public string Category { get; set; }
        public NotificationStatus? Status { get; set; }
        public NotificationType? Type { get; set; }
    } 

    public class Handler : IRequestHandler<Request, QueryResponse>
    {
        private readonly INotificationsService _notificationsService;

        public Handler(INotificationsService notificationsService)
        {
            _notificationsService = notificationsService;
        }

        public async Task<QueryResponse> Handle(Request request, CancellationToken cancellation)
        {
            var notifications = await _notificationsService.GetAllAsync(request.Identity.Username, request.Category, request.Status, request.Type, cancellation);

            return QueryResponse.Success(notifications);
        }
    }
}
