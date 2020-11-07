using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.MediatR;
using rbkApiModules.Infrastructure.MediatR.SqlServer;
using System;
using System.Threading.Tasks;

namespace rbkApiModules.Comments
{
    public class GetComments
    {
        public class Command : IRequest<QueryResponse>
        {
            public Guid EntityId { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
            }
        }

        public class Handler : BaseQueryHandler<Command, DbContext>
        {
            private readonly ICommentsService _commentsService;

            public Handler(DbContext context, IHttpContextAccessor httpContextAccessor, ICommentsService commentsService)
                : base(context, httpContextAccessor)
            {
                _commentsService = commentsService;
            }

            protected override async Task<object> ExecuteAsync(Command request)
            {
                return await _commentsService.GetComments(request.EntityId);
            }
        }
    }
}
