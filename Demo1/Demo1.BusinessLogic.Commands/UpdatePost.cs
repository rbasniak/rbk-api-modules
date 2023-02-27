using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Demo1.Database.Domain;
using Demo1.Models.Domain.Demo;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Relational;
using rbkApiModules.Commons.Core.CQRS;
using rbkApiModules.Commons.Core.Localization;

namespace Demo1.BusinessLogic.Commands;

public class UpdatePost
{
    public class Request : CreatePost.Request, IHasReadingModel<Models.Read.Post>
    {
        public Guid Id { get; set; }
    }

    public class Validator: AbstractValidator<Request>
    {
        public Validator(DbContext context, ILocalizationService localization)
        {
            RuleFor(x => x.Id).MustExistInDatabase<Request, Post>(context, localization);
        }
    }

    public class Handler : IRequestHandler<Request, AuditableCommandResponse>
    {
        private readonly DatabaseContext _context;

        public Handler(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<AuditableCommandResponse> Handle(Request request, CancellationToken cancellation)
        {
            var post = await _context.Posts
                .Include(x => x.Blog)
                .Include(x => x.Author)
                .FirstAsync(x => x.Id == request.Id);

            post.Update(request.Title, request.Body);

            await _context.SaveChangesAsync();

            return AuditableCommandResponse.Success(post, post.Id, post.BlogId);
        }
    }
}