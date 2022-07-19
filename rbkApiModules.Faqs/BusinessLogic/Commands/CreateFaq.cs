using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.MediatR.Core;
using rbkApiModules.Infrastructure.MediatR.SqlServer;
using System;
using System.Threading.Tasks;

namespace rbkApiModules.Faqs
{
    public class CreateFaq
    {
        public class Command : IRequest<CommandResponse>
        {
            public string Tag { get; set; }
            public string Question { get; set; }
            public string Answer { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Tag)
                    .IsRequired();

                RuleFor(x => x.Question)
                    .IsRequired();

                RuleFor(x => x.Answer)
                    .IsRequired();
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
                var faq = new Faq(request.Tag, request.Question, request.Answer);

                _context.Add(faq);

                await _context.SaveChangesAsync();

                return (faq.Id, faq);
            }
        }
    }
}