using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.MediatR.SqlServer;
using rbkApiModules.Utilities.Extensions;
using System;
using System.Threading.Tasks;
using rbkApiModules.Infrastructure.MediatR.Core;

namespace rbkApiModules.Comments
{
    public class CommentEntity
    {
        public class Command : IRequest<CommandResponse>
        {
            public Guid EntityId { get; set; }
            public Guid? ParentId { get; set; }
            public string Comment { get; set; }
        }

        public class Validator: AbstractValidator<Command>  
        {
            public Validator(DbContext context, IHttpContextAccessor httpContextAccessor)
            {
                CascadeMode = CascadeMode.Stop;

                RuleFor(x => x.ParentId)
                    .MustExistInDatabase<Command, Comment>(context).WithMessage("Não foi possível localizar o comentário");

                RuleFor(x => x.Comment)
                    .IsRequired()
                    .WithName("Mensagem");
            }
        }

        public class Handler : BaseCommandHandler<Command>
        {
            private readonly ICommentsService _commentsService;
            private readonly DbContext _context;

            public Handler(DbContext context, IHttpContextAccessor httpContextAccessor, ICommentsService commentsService)
                : base(httpContextAccessor)
            {
                _commentsService = commentsService;
                _context = context;
            }

            protected override async Task<object> ExecuteAsync(Command request)
            {
                Comment parent = null;

                if (request.ParentId != null)
                {
                    parent = await _context.Set<Comment>().FirstAsync(x => x.Id == request.ParentId);
                }

                var comment = new Comment(request.EntityId, _httpContextAccessor.GetUsername(), parent, request.Comment);

                _context.Add(comment);

                var saved = await _context.SaveChangesAsync();

                var result = await _commentsService.GetComments(request.EntityId);

                return result;
            }
        }
    }
}
