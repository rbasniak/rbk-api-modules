using Demo2.Samples.Eventsourcing.EventOrientedChanges.Database.Repositories;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Events.Entities.Attachments;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Models;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Infrastructure.Models;
using Demo2.Samples.Relational.Database;
using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;

namespace Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Commands.Attachments;

// https://itnext.io/implementing-event-store-in-c-8a27138cc90

public class AddAttachmentToChangeRequest
{
    public class PreRequest : AuthenticatedRequest, IRequest<CommandResponse>
    {
        public Guid ChangeRequestId { get; set; }
        public string Name { get; set; }
        public string File { get; set; }
        public string Filename { get; set; }
    }

    public class Request : ICommand<ChangeRequest>
    {
        public Guid ChangeRequestId { get; set; }
        public string Name { get; set; }
        public string Filename { get; set; }
        public string Path { get; set; }
        public int Size { get; set; }
        public Guid TypeId { get; set; }
        public string Username { get; set; }

        public IEnumerable<IDomainEvent<ChangeRequest>> ExecuteOn(ChangeRequest entity)
        {
            var results = new List<IDomainEvent<ChangeRequest>>()
            {
                new AttachmentAddedToChangeRequest.V1(Username, ChangeRequestId, Name, Filename, Path, TypeId, Size)
            };

            return results;
        }
    }

    public class Validator : AbstractValidator<Request>
    {

    }

    public class Handler : IRequestHandler<PreRequest, CommandResponse>
    {
        private readonly RelationalContext _context;
        private readonly IChangeRequestRepository _changeRequestRepository;

        public Handler(IChangeRequestRepository changeRequestRepository, RelationalContext context)
        {
            _context = context;
            _changeRequestRepository = changeRequestRepository;
        }

        public async Task<CommandResponse> Handle(PreRequest preRequest, CancellationToken cancellationToken)
        {
            var changeRequest = await _changeRequestRepository.FindAsync(preRequest.ChangeRequestId);

            var type = _context.Set<Relational.Domain.Models.AttachmentType>().First();
            var size = preRequest.File.Length;
            var path = "TODO...";

            var command = new Request
            {
                ChangeRequestId = preRequest.ChangeRequestId,
                TypeId = type.Id,
                Size = size,
                Name = preRequest.Name,
                Filename = preRequest.Filename,
                Path = path,
                Username = preRequest.Identity.Username,
            };

            command.ExecuteOn(changeRequest);

            return CommandResponse.Success();
        }
    }
}