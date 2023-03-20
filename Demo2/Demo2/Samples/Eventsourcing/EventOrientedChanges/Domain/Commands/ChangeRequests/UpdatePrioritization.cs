using Demo2.Samples.Eventsourcing.EventOrientedChanges.Database.Repositories;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Events.Entities.ChangeRequests;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Models;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Infrastructure.Models;
using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;

namespace Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Commands.ChangeRequests;

// https://itnext.io/implementing-event-store-in-c-8a27138cc90

public class UpdateChangeRequestPrioritization
{
    public class Request : AuthenticatedRequest, IRequest<CommandResponse>, ICommand<ChangeRequest>
    {
        public Guid ChangeRequestId { get; set; }
        public int G { get; set; }
        public int U { get; set; }
        public int T { get; set; }

        public IEnumerable<IDomainEvent<ChangeRequest>> ExecuteOn(ChangeRequest entity)
        {
            var results = new List<IDomainEvent<ChangeRequest>>();

            if (entity.Prioritization.Gravity != G ||
                entity.Prioritization.Urgency != U ||
                entity.Prioritization.Tendency != T)
            {
                results.Add(new ChangeRequestPrioritizationUpdated.V1(Identity.Username, ChangeRequestId, G, U, T));
            }

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