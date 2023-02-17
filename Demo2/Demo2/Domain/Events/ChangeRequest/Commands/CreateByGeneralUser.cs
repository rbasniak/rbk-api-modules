using Demo2.Domain.Events.Infrastructure;
using Demo2.Domain.Events.Repositories;
using Demo2.Domain.Models;
using Demo2.Infrastructure.EventSourcing.Database.Repositories;
using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.CQRS;
using System.Diagnostics.Tracing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

// https://itnext.io/implementing-event-store-in-c-8a27138cc90

public class CreateChangeRequestByGeneralUser
{
    public class Request : IRequest<CommandResponse>, IHasReadingModel<ChangeRequest>
    {
        public OperationType Mode => OperationType.Add;

        public string RequestedBy { get; set; }
        public string CreatedBy { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
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
            var newId = await _changeRequestRepository.Create(request.RequestedBy, request.CreatedBy, request.Description, request.Title);

            var changeRequest = await _changeRequestRepository.FindAsync(newId);

            return CommandResponse.Success(changeRequest);
        }
    }
}