using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core.Utilities.Localization;

namespace rbkApiModules.Comments.Core;

public class CommentEntity
{
    public class Request : AuthenticatedRequest, IRequest<CommandResponse>
    {
        public Guid EntityId { get; set; }
        public Guid? ParentId { get; set; }
        public string Comment { get; set; }
    }

    public class Validator: AbstractValidator<Request>  
    {
        public Validator(ILocalizationService localization, ICommentsService commentsService)
        {
            RuleFor(x => x.Comment)
                .IsRequired(localization)
                .WithName(localization.GetValue(CommentMessages.Fields.Comment));

            When(x => x.ParentId != null, () => 
            {
                RuleFor(x => x.ParentId)
                    .MustAsync(async (command, parentId, cancellation) => await commentsService.ExistsAsync(command.Identity.Tenant, parentId.Value, cancellation))
                    .WithMessage(localization.GetValue(CommentMessages.Validation.CouldNotFindParentComment));
            });
        }
    }

    public class Handler : IRequestHandler<Request, CommandResponse>
    {
        private readonly ICommentsService _commentsService;

        public Handler(ICommentsService commentsService)
        {
            _commentsService = commentsService;
        }

        public async Task<CommandResponse> Handle(Request request, CancellationToken cancellation)
        { 
            await _commentsService.CreateAsync(request.Identity.Tenant, request.Identity.Username, request.Comment, request.EntityId, request.ParentId, cancellation);

            var comments = await _commentsService.GetAllAsync(request.Identity.Tenant, request.EntityId, cancellation);

            return CommandResponse.Success(comments);
        }
    }
}
