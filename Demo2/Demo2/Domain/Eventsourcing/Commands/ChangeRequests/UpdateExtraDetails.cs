using Demo2.Domain.Events.Infrastructure;
using Demo2.Domain.Events.Repositories;
using Demo2.EventSourcing;
using Demo2.Infrastructure.EventSourcing.Database.Repositories;
using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.CQRS;
using System.Diagnostics.Tracing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

// https://itnext.io/implementing-event-store-in-c-8a27138cc90

public class UpdateChangeRequestExtraDetails
{
    public class Request : AuthenticatedRequest, IRequest<CommandResponse>, ICommand<ChangeRequest>
    {
        public Guid ChangeRequestId { get; set; }
        public string StatusSgm { get; set; }
        public string Comments { get; set; }

        public IEnumerable<IDomainEvent<ChangeRequest>> ExecuteOn(ChangeRequest entity)
        {
            var results = new List<IDomainEvent<ChangeRequest>>();

            if (StatusSgm != entity.StatusSgm) results.Add(new ChangeRequestSgmStatusUpdated.V1(Identity.Username, ChangeRequestId, StatusSgm));
            if (Comments != entity.Comments) results.Add(new ChangeRequestCommentsUpdated.V1(Identity.Username, ChangeRequestId, Comments));

            return results;
        } 
    }

    public class Validator : AbstractValidator<Request>
    {

    }

    public class Handler : IRequestHandler<Request, CommandResponse>
    {
        private readonly IChangeRequestRepository _changeRequestRepository;

        public Handler(IChangeRequestRepository changeRequestRepository)
        {
            _changeRequestRepository = changeRequestRepository;
        }

        public async Task<CommandResponse> Handle(Request request, CancellationToken cancellationToken)
        {
            var changeRequest = await _changeRequestRepository.FindAsync(request.ChangeRequestId);

            request.ExecuteOn(changeRequest);

            return CommandResponse.Success();
        }
    }
}