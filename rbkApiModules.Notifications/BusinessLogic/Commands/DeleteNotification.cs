using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.MediatR.SqlServer;
using System;
using System.Threading.Tasks;
using rbkApiModules.Infrastructure.MediatR.Core;
using rbkApiModules.Notifications;
using System.Linq;
using rbkApiModules.Utilities.Extensions;

namespace rbkApiModules.Comments
{
    public class DeleteNotification
    {
        public class Command : IRequest<CommandResponse>
        {
            [MustExist(typeof(Notification))]
            public Guid[] NotificationIds { get; set; }
        }

        public class Handler : BaseCommandHandler<Command, DbContext>
        {
            public Handler(DbContext context, IHttpContextAccessor httpContextAccessor)
                : base(context, httpContextAccessor)
            {
            }

            protected override async Task<(Guid? entityId, object result)> ExecuteAsync(Command request)
            {
                var user = _httpContextAccessor.GetUsername();

                var notifications = await _context.Set<Notification>()
                    .Where(x => x.User.ToLower() == user && 
                        request.NotificationIds.Any(y => x.Id == y)).ToListAsync();

                _context.RemoveRange(notifications);

                await _context.SaveChangesAsync();

                return (null, notifications.Select(x => x.Id));
            }
        }
    }
}
