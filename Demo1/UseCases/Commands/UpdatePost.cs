using Demo1.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Demo1.UseCases.Commands;

public class UpdatePost : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("/posts", async (Request request, IDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var result = await dispatcher.SendAsync(request, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("Update Post")
        .WithTags("Posts");
    }

    public class Request : AuthenticatedRequest, ICommand
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string UniqueInApplication { get; set; } = string.Empty;
        public string UniqueInTenant { get; set; } = string.Empty;
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator(DbContext context)
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .MustAsync(async (id, cancellationToken) =>
                    await context.Set<Post>().AnyAsync(x => x.Id == id, cancellationToken))
                .WithMessage("Post with the specified ID does not exist.");

            RuleFor(x => x.Title)
                .NotEmpty()
                .MinimumLength(2)
                .MaximumLength(16)
                .MustAsync(async (request, title, cancellationToken) =>
                    !await context.Set<Post>().AnyAsync(x => x.Title == title && x.Id != request.Id, cancellationToken))
                .WithMessage("A post with this title already exists.");

            RuleFor(x => x.Body)
                .NotEmpty()
                .MinimumLength(32)
                .MaximumLength(4096);

            RuleFor(x => x.UniqueInApplication)
                .NotEmpty()
                .MinimumLength(1)
                .MaximumLength(32)
                .MustAsync(async (request, uniqueInApplication, cancellationToken) =>
                    !await context.Set<Post>().AnyAsync(x => x.UniqueInApplication == uniqueInApplication && x.Id != request.Id, cancellationToken))
                .WithMessage("A post with this unique application value already exists.");

            RuleFor(x => x.UniqueInTenant)
                .NotEmpty()
                .MinimumLength(1)
                .MaximumLength(32)
                .MustAsync(async (request, uniqueInTenant, cancellationToken) =>
                    !await context.Set<Post>().AnyAsync(x => x.UniqueInTenant == uniqueInTenant && x.TenantId == request.Identity.Tenant && x.Id != request.Id, cancellationToken))
                .WithMessage("A post with this unique tenant value already exists.");
        }
    }

    public class Handler(DbContext _context) : ICommandHandler<Request>
    {
        public async Task<CommandResponse> HandleAsync(Request request, CancellationToken cancellationToken)
        {
            var post = await _context.Set<Post>()
                .FirstAsync(x => x.Id == request.Id, cancellationToken);

            post.Update(
                request.Title,
                request.Body,
                request.UniqueInApplication,
                request.UniqueInTenant
            );

            await _context.SaveChangesAsync(cancellationToken);

            return CommandResponse.Success();
        }
    }
} 