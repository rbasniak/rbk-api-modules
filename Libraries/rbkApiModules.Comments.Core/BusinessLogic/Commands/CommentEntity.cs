using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;

namespace rbkApiModules.Comments.Core;

public class CommentEntity
{
    public class Command : AuthenticatedRequest, IRequest<CommandResponse>
    {
        public Guid EntityId { get; set; }
        public Guid? ParentId { get; set; }
        public string Comment { get; set; }
    }

    public class Validator: AbstractValidator<Command>  
    {
        public Validator(ILocalizationService localization, ICommentsService commentsService)
        {
            RuleFor(x => x.Comment)
                .IsRequired(localization)
                .WithName(localization.GetValue("Message"));

            When(x => x.ParentId != null, () => 
            {
                RuleFor(x => x.ParentId)
                    .MustAsync(async (command, parentId, cancellation) => await commentsService.ExistsAsync(command.Identity.Tenant, parentId.Value, cancellation))
                    .WithMessage(localization.GetValue("Could not find parent comment"));
            });
        }
    }

    public class Handler : IRequestHandler<Command, CommandResponse>
    {
        private readonly ICommentsService _commentsService;

        public Handler(ICommentsService commentsService)
        {
            _commentsService = commentsService;
        }

        public async Task<CommandResponse> Handle(Command request, CancellationToken cancellation)
        { 
            await _commentsService.CreateAsync(request.Identity.Tenant, request.Identity.Username, request.Comment, request.EntityId, request.ParentId, cancellation);

            var comments = await _commentsService.GetAllAsync(request.Identity.Tenant, request.EntityId, cancellation);

            return CommandResponse.Success(comments);
        }
    }
}
