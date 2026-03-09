using Demo1.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Demo1.UseCases.Commands;

public interface ICreatePostRequest
{    
    string Title { get; set; }
    string Body { get; set; }
    Guid AuthorId { get; set; }
    Guid BlogId { get; set; }
    string UniqueInApplication { get; set; }
    string UniqueInTenant { get; set; }
}

public class CreatePostRequest : ICreatePostRequest, ICommand
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public Guid BlogId { get; set; }
    public string UniqueInApplication { get; set; } = string.Empty;
    public string UniqueInTenant { get; set; } = string.Empty;
}

public class CreatePost : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/posts", async (CreatePostRequest request, IDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var result = await dispatcher.SendAsync(request, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("Create Post")
        .WithTags("Posts");
    }

    public class Request : AuthenticatedRequest, ICommand, ICreatePostRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public Guid AuthorId { get; set; }
        public Guid BlogId { get; set; }
        public string UniqueInApplication { get; set; } = string.Empty;
        public string UniqueInTenant { get; set; } = string.Empty;
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator(DbContext context)
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MinimumLength(2)
                .MaximumLength(16)
                .MustAsync(async (title, cancellationToken) =>
                    !await context.Set<Post>().AnyAsync(x => x.Title == title, cancellationToken))
                .WithMessage("A post with this title already exists.");

            RuleFor(x => x.Body)
                .NotEmpty()
                .MinimumLength(32)
                .MaximumLength(4096);

            RuleFor(x => x.AuthorId)
                .NotEmpty()
                .MustAsync(async (authorId, cancellationToken) =>
                    await context.Set<Author>().AnyAsync(x => x.Id == authorId, cancellationToken))
                .WithMessage("The specified author does not exist.");

            RuleFor(x => x.BlogId)
                .NotEmpty()
                .MustAsync(async (blogId, cancellationToken) =>
                    await context.Set<Blog>().AnyAsync(x => x.Id == blogId, cancellationToken))
                .WithMessage("The specified blog does not exist.");

            RuleFor(x => x.UniqueInApplication)
                .NotEmpty()
                .MinimumLength(1)
                .MaximumLength(32)
                .MustAsync(async (uniqueInApplication, cancellationToken) =>
                    !await context.Set<Post>().AnyAsync(x => x.UniqueInApplication == uniqueInApplication, cancellationToken))
                .WithMessage("A post with this unique application value already exists.");

            RuleFor(x => x.UniqueInTenant)
                .NotEmpty()
                .MinimumLength(1)
                .MaximumLength(32)
                .MustAsync(async (request, uniqueInTenant, cancellationToken) =>
                    !await context.Set<Post>().AnyAsync(x => x.UniqueInTenant == uniqueInTenant && x.TenantId == request.Identity.Tenant, cancellationToken))
                .WithMessage("A post with this unique tenant value already exists.");
        }
    }

    public class Handler(DbContext _context) : ICommandHandler<Request>
    {
        public async Task<CommandResponse> HandleAsync(Request request, CancellationToken cancellationToken)
        {
            var author = await _context.Set<Author>()
                .FirstAsync(x => x.Id == request.AuthorId, cancellationToken);

            var blog = await _context.Set<Blog>()
                .FirstAsync(x => x.Id == request.BlogId, cancellationToken);

            var post = new Post(
                request.Identity.Tenant,
                blog,
                author,
                request.Title,
                request.Body,
                request.UniqueInTenant,
                request.UniqueInApplication
            );

            await _context.AddAsync(post, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return CommandResponse.Success();
        }
    }
} 