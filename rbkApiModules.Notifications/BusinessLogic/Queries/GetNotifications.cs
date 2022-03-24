using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.MediatR.SqlServer;
using System.Threading.Tasks;
using rbkApiModules.Infrastructure.MediatR.Core;
using rbkApiModules.Notifications.Models;
using rbkApiModules.Utilities.Extensions;

namespace rbkApiModules.Notifications
{
    public class GetNotifications
    {
        public class Command : IRequest<QueryResponse>
        {
            public string Category { get; set; }
            public NotificationStatus? Status { get; set; }
            public NotificationType? Type { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
            }
        }

        public class Handler : BaseQueryHandler<Command, DbContext>
        {
            private readonly INotificationsService _notificationsService;

            public Handler(DbContext context, IHttpContextAccessor httpContextAccessor, INotificationsService notificationsService)
                : base(context, httpContextAccessor)
            {
                _notificationsService = notificationsService;
            }

            protected override async Task<object> ExecuteAsync(Command request)
            {
                return await _notificationsService.GetNotifications(_httpContextAccessor.GetUsername(), request.Category, request.Status, request.Type);
            }
        }
    }
}
