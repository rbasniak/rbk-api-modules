using Demo2.Domain.Events.Infrastructure;
using Demo2.Domain.Events.Repositories;
using Demo2.Domain.Models;
using Demo2.Infrastructure.EventSourcing.Database.Repositories;
using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using System.Diagnostics.Tracing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

// https://itnext.io/implementing-event-store-in-c-8a27138cc90

public partial class ChangeRequestCommands
{
    public class CreateByGeneralUser
    {
        public class Request : IRequest<CommandResponse>
        {
            public string RequestedBy { get; set; }
            public string CreatedBy { get; set; }
            public string Description { get; set; }
            public string Title { get; set; }
        }

        public class Validator: AbstractValidator<Request>
        {

        }

        public class Handler : IRequestHandler<Request, CommandResponse>
        {
            private readonly IEventStore _eventStore;
            private readonly IChangeRequestRepository _changeRequestRepository;

            public Handler(IEventStore eventStore, IChangeRequestRepository changeRequestRepository)
            {
                _eventStore = eventStore;
                _changeRequestRepository = changeRequestRepository;
            }

            public async Task<CommandResponse> Handle(Request request, CancellationToken cancellationToken)
            {
                var changeRequestId = await _changeRequestRepository.Create(request.RequestedBy, request.CreatedBy, request.Description, request.Title);

                var changeRequest = await _changeRequestRepository.FindAsync(changeRequestId);

                return CommandResponse.Success(changeRequest);
            }
        }
    }
}