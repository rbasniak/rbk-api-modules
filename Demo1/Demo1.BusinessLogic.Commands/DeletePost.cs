using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Demo1.Database.Domain;
using Demo1.Models.Domain.Demo;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Relational;
using rbkApiModules.Commons.Core.CQRS;

namespace Demo1.BusinessLogic.Commands;

public class DeletePost
{
    public class Command : CreatePost.Command, IHasReadingModel<Models.Read.Post>
    {
        public new OperationType Mode => OperationType.Remove;

        public Guid Id { get; set; }
    }

    public class Validator: AbstractValidator<Command>
    {
        public Validator(DatabaseContext context)
        {
            RuleFor(x => x.Id).MustExistInDatabase<Command, Post>(context);
        }
    }

    public class Handler : IRequestHandler<Command, AuditableCommandResponse>
    {
        private readonly DatabaseContext _context;

        public Handler(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<AuditableCommandResponse> Handle(Command command, CancellationToken cancellation)
        {
            var post = await _context.Posts
                .FirstAsync(x => x.Id == command.Id);

            _context.Remove(post);

            await _context.SaveChangesAsync();

            return AuditableCommandResponse.Success(post, post.Id, post.BlogId);
        }
    }
}