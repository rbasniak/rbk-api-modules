using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using rbkApiModules.Infrastructure.MediatR.Core;
using rbkApiModules.Infrastructure.MediatR.SqlServer;

namespace rbkApiModules.Faqs
{
    public class DeleteFaq
    {
        public class Command : IRequest<CommandResponse>
        {
            public Guid Id { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator(DbContext context)
            {
                RuleFor(x => x.Id)
                    .MustExistInDatabase<Command, Faq>(context);
            }
        }

        public class Handler : BaseCommandHandler<Command, DbContext>
        {
            public Handler(DbContext context, IHttpContextAccessor httpContextAccessor)
                : base(context, httpContextAccessor)
            {
            }

            protected override async Task<(Guid? entityId, object result)> ExecuteAsync(Command request)
            {
                var faq = await _context.Set<Faq>().FirstAsync(x => x.Id == request.Id);

                _context.Remove(faq);

                var saved = await _context.SaveChangesAsync();

                return (faq.Id, faq);
            }
        }
    }
}