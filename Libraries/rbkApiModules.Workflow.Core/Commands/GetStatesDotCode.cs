using AutoMapper;
using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core.Utilities.Localization;

namespace rbkApiModules.Workflow.Core;

public class GetStatesDotCode
{
    public class Request : IRequest<QueryResponse>
    {
        public Guid GroupId { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator(IStatesService statesService, ILocalizationService localization)
        {
            RuleFor(x => x.GroupId)
                .MustAsync(async (command, id, cancellation) => (await statesService.FindGroup(id, cancellation)) != null)
                .WithMessage(localization.GetValue(WorkflowMessages.Validation.StateGroupNotFound));
        }
    }

    public class Handler: IRequestHandler<Request, QueryResponse>
    {
        protected readonly IMapper _mapper;
        protected readonly IStatesService _statesService;

        public Handler(IStatesService statesService, IMapper mapper)
        {
            _mapper = mapper;
            _statesService = statesService;
        }

        public async Task<QueryResponse> Handle(Request request, CancellationToken cancellationToken)
        {
            var states = await _statesService.GetStates(cancellationToken);

            var code = DotCodeGenerator.GenerateCode(states);

            return QueryResponse.Success(new Response()
            {
                Code = code
            });
        }
    }

    public class Response
    {
        required public string Code { get; set; }
    }
}
