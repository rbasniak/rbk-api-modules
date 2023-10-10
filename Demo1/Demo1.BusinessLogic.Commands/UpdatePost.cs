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
    public class Request : CreatePost.Request 
    {
        public Guid Id { get; set; }
    }

    public class Validator: AbstractValidator<Request>, IDomainEntityValidator<Post>
    {
        private readonly DatabaseContext _context;
        private readonly ILocalizationService _localization;

        public Validator(DatabaseContext context, ILocalizationService localization)
        {
            _localization = localization;
            _context = context;
        }
    }

    public class Handler : IRequestHandler<Request, CommandResponse>
    {
        private readonly DatabaseContext _context;

        public Handler(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(Request request, CancellationToken cancellation)
        {
            var post = await _context.Posts
                .Include(x => x.Blog)
                .Include(x => x.Author)
                .FirstAsync(x => x.Id == request.Id);

            post.Update(request.Title, request.Body, request.UniqueInApplication, request.UniqueInTenant);

            await _context.SaveChangesAsync();

            return CommandResponse.Success();
        }
    }
}