using Demo2.Domain.Events.Infrastructure;
using Demo2.Domain.Events.Repositories;
using Demo2.EventSourcing;
using Demo2.Infrastructure.EventSourcing.Database.Repositories;
using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.CQRS;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

// https://itnext.io/implementing-event-store-in-c-8a27138cc90

public class CreateChangeRequestByGeneralUser
{
    public class Request : AuthenticatedRequest, IRequest<CommandResponse>, ICommand<ChangeRequest>
    {
        public Request()
        {
            ChangeRequestId = Guid.NewGuid();    
        }

        public Guid PlatformId { get; set; }
        public Guid TypeId { get; set; }
        public Guid PriorityId { get; set; }
        public Guid SourceId { get; set; }
        public string Description { get; set; }
        public string Justification { get; set; }
        public string RequestedBy { get; set; }
        public string SourceNumber { get; set; }

        [JsonIgnore]
        public Guid ChangeRequestId { get; }

        public IEnumerable<IDomainEvent<ChangeRequest>> ExecuteOn(ChangeRequest entity)
        {
            var results = new List<IDomainEvent<ChangeRequest>> 
            {
                new ChangeRequestCreatedByGeneralUser.V1(Identity.Username, ChangeRequestId)
            };

            if (PlatformId != entity.PlatformId) results.Add(new ChangeRequestPlatformUpdated.V1(Identity.Username, ChangeRequestId, PlatformId));
            if (TypeId != entity.TypeId) results.Add(new ChangeRequestTypeUpdated.V1(Identity.Username, ChangeRequestId, TypeId));
            if (PriorityId != entity.PriorityId) results.Add(new ChangeRequestPriorityUpdated.V1(Identity.Username, ChangeRequestId, PriorityId));
            if (SourceId != entity.SourceId) results.Add(new ChangeRequestSourceUpdated.V1(Identity.Username, ChangeRequestId, SourceId));
            if (Description != entity.Description) results.Add(new ChangeRequestDescriptionUpdated.V1(Identity.Username, ChangeRequestId, Description));
            if (Justification != entity.Justification) results.Add(new ChangeRequestJustificationUpdated.V1(Identity.Username, ChangeRequestId, Justification));
            if (RequestedBy != entity.RequestedBy) results.Add(new ChangeRequestRequestedByUpdated.V1(Identity.Username, ChangeRequestId, RequestedBy));
            if (SourceNumber != entity.SourceNumber) results.Add(new ChangeRequestSourceNumberUpdated.V1(Identity.Username, ChangeRequestId, SourceNumber));
            if (RequestedBy != entity.CurrentOwner) results.Add(new ChangeRequestCurrentOwnerUpdated.V1(Identity.Username, ChangeRequestId, RequestedBy));

            var temp = results.Single(x => x is ChangeRequestTypeUpdated.V1);
            var temp1 = (ChangeRequestTypeUpdated.V1)temp;
            if (temp1.TypeId == Guid.Empty) Debugger.Break();

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