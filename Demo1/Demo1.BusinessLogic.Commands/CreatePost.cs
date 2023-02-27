using FluentValidation;
using MediatR;
using System.Text.Json.Serialization;
using Demo1.Database.Domain;
using Demo1.Models.Domain.Demo;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Relational;
using rbkApiModules.Commons.Core.CQRS;
using rbkApiModules.Commons.Core.Localization;

namespace Demo1.BusinessLogic.Commands;

public class CreatePost
{
    public class Request : IRequest<AuditableCommandResponse>, IHasReadingModel<Models.Read.Post>
    {
        [JsonIgnore]
        public OperationType Mode => OperationType.AddOrUpdate;

        public string Title { get; set; }
        public string Body { get; set; }
        public Guid BlogId { get; set; }
        public Guid AuthorId { get; set; }
    }

    public class Validator: AbstractValidator<Request>
    {
        public Validator(DatabaseContext context, ILocalizationService localization)
        {
            RuleFor(x => x.BlogId)
                .MustExistInDatabase<Request, Blog>(context, localization);

            RuleFor(x => x.AuthorId)
                .MustExistInDatabase<Request, Author>(context, localization);
        }
    }

    public class Handler : IRequestHandler<Request, AuditableCommandResponse>
    {
        private readonly DatabaseContext _context;

        public Handler(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<AuditableCommandResponse>Handle(Request request, CancellationToken cancellation)
        {
            var blog = await _context.Blogs.FindAsync(request.BlogId);
            
            var author = await _context.Authors.FindAsync(request.AuthorId);

            var post = new Post(blog, author, request.Title, request.Body);

            _context.Add(post);

            _context.SaveChanges();

            return AuditableCommandResponse.Success(post, post.Id, blog.Id);
        }
    }
}