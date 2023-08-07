using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core;
using Demo3.Models;
using Microsoft.EntityFrameworkCore;

namespace Demo3.Commands
{
    public class CreateProject
    {
        public class Request : IRequest<CommandResponse>
        {
            public string Name { get; set; }
            public string Code { get; set; }
            public string Mdb { get; set; }
        }

        public class Validator : AbstractValidator<Request>
        {
            public Validator(ILocalizationService localization)
            {
                RuleFor(x => x.Name)
                    .IsRequired(localization)
                    .MustNotBeNull(localization)
                    .WithName("Name");

                RuleFor(x => x.Code)
                    .IsRequired(localization)
                    .MustNotBeNull(localization)
                    .WithName("Code");

                RuleFor(x => x.Mdb)
                    .IsRequired(localization)
                    .MustNotBeNull(localization)
                    .WithName("Mdb");
            }
        }

        public class Handler : IRequestHandler<Request, CommandResponse>
        {
            private readonly DbContext _context;
            public Handler(DbContext context)
            {
                _context = context;
            }

            public async Task<CommandResponse> Handle(Request request, CancellationToken cancellationToken)
            {
                var project = new Project(request.Name, request.Code, request.Mdb);

                await _context.AddAsync(project, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                return CommandResponse.Success(project);
            }
        }
    }
}
