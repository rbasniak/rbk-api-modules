using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.MediatR.Core;
using rbkApiModules.Infrastructure.MediatR.SqlServer;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace rbkApiModules.Authentication
{
    public class ResendEmailConfirmation
    {
        public class Command : IRequest<CommandResponse>
        {
            public string Email { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly DbContext _context;

            public Validator(DbContext context)
            {
                _context = context;

                CascadeMode = CascadeMode.Stop;

                RuleFor(x => x.Email)
                    .IsRequired()
                    .MustBeEmail()
                    .MustAsync(EmailBeRegistered).WithMessage("E-mail não cadastrado")
                    .MustAsync(EmailBeUnconfirmed).WithMessage("E-mail já confirmado");
            }

            private async Task<bool> EmailBeRegistered(Command command, string email, CancellationToken cancelation)
            {
                return await _context.Set<BaseUser>().AnyAsync(x => EF.Functions.Like(x.Email, email), cancellationToken: cancelation);
            }

            private async Task<bool> EmailBeUnconfirmed(Command command, string email, CancellationToken cancelation)
            {
                return await _context.Set<BaseUser>().AnyAsync(x => EF.Functions.Like(x.Email, command.Email) && x.ActivationCode != null,
                        cancellationToken: cancelation);
            }
        }

        public class Handler : BaseCommandHandler<Command, DbContext>
        {
            private readonly IAuthenticationMailsService _mailingService;

            public Handler(DbContext context, IHttpContextAccessor httpContextAccessor, IAuthenticationMailsService mailingService) : base(context, httpContextAccessor)
            {
                _mailingService = mailingService;
            }

            protected override async Task<(Guid? entityId, object result)> ExecuteAsync(Command request)
            {
                var user = await _context.Set<BaseUser>()
                    .SingleAsync(x => EF.Functions.Like(x.Email, request.Email));

                _mailingService.SendConfirmationMail(user.DisplayName, user.Email, user.ActivationCode);

                return (null, null);
            }
        }
    }
}
