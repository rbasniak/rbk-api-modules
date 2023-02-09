using MediatR;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Notifications.Core;

public class DeleteNotification
{
    public class Command : AuthenticatedRequest, IRequest<CommandResponse>
    {
        public Guid[] NotificationIds { get; set; }
    }

    public class Handler : IRequestHandler<Command, CommandResponse>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly INotificationsService _notificationsService;

        public Handler(INotificationsService notificationsService, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _notificationsService = notificationsService;
        }

        public async Task<CommandResponse> Handle(Command request, CancellationToken cancellation)
        {
            await _notificationsService.DeleteAsync(request.Identity.Username, request.NotificationIds, cancellation);

            throw new NotImplementedException("Pq esta retornando os ids?");
            // return (null, notifications.Select(x => x.Id));

            return CommandResponse.Success();
        }
    }
}
