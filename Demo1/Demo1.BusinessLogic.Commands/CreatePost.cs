using FluentValidation;
using MediatR;
using System.Text.Json.Serialization;
using Demo1.Database.Domain;
using Demo1.Models.Domain.Demo;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Relational;
using rbkApiModules.Commons.Core.CQRS;

namespace Demo1.BusinessLogic.Commands;

public class CreatePost
{
    public class Command : IRequest<AuditableCommandResponse>, IHasReadingModel<Models.Read.Post>
    {
        [JsonIgnore]
        public OperationType Mode => OperationType.AddOrUpdate;

        public string Title { get; set; }
        public string Body { get; set; }
        public Guid BlogId { get; set; }
        public Guid AuthorId { get; set; }
    }

    public class Validator: AbstractValidator<Command>
    {
        public Validator(DatabaseContext context)
        {
            RuleFor(x => x.BlogId)
                .MustExistInDatabase<Command, Blog>(context);

            RuleFor(x => x.AuthorId)
                .MustExistInDatabase<Command, Author>(context);
        }
    }

    public class Handler : IRequestHandler<Command, AuditableCommandResponse>
    {
        private readonly DatabaseContext _context;

        public Handler(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<AuditableCommandResponse>Handle(Command command, CancellationToken cancellation)
        {
            var blog = await _context.Blogs.FindAsync(command.BlogId);
            
            var author = await _context.Authors.FindAsync(command.AuthorId);

            var post = new Post(blog, author, command.Title, command.Body);

            _context.Add(post);

            _context.SaveChanges();

            return AuditableCommandResponse.Success(post, post.Id, blog.Id);
        }
    }
}