using Demo2.Samples.Eventsourcing.EventOrientedChanges.Database.Repositories;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Events.Entities.Fics;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Models;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Infrastructure.Models;
using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;

namespace Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Commands.Fics;

// https://itnext.io/implementing-event-store-in-c-8a27138cc90

public class UpdateFicOnChangeRequest
{
    public class Request : AuthenticatedRequest, IRequest<CommandResponse>, ICommand<ChangeRequest>
    {
        public Guid ChangeRequestId { get; set; }
        public Guid FicId { get; set; }
        public string Name { get; set; }
        public Guid CategoryId { get; set; }

        public IEnumerable<IDomainEvent<ChangeRequest>> ExecuteOn(ChangeRequest entity)
        {
            var results = new List<IDomainEvent<ChangeRequest>>();

            var fic = entity.Fics.First(x => x.Id == FicId);

            if (fic.Name != Name) results.Add(new FicNameUpdatedOnChangeRequest.V1(Identity.Username, ChangeRequestId, FicId, Name));
            if (fic.CategoryId != CategoryId) results.Add(new FicCategoryUpdatedOnChangeRequest.V1(Identity.Username, ChangeRequestId, FicId, CategoryId));

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