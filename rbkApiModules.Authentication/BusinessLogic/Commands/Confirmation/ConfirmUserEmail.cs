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
    public class ConfirmUserEmail
    {
        public class Command : IRequest<CommandResponse>
        {
            public string Email { get; set; }
            public string ActivationCode { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly DbContext _context;

            public Validator(DbContext context)
            {
                _context = context;

                CascadeMode = CascadeMode.Stop;

                RuleFor(a => a.ActivationCode)
                     .MustAsync(BeValidPair).WithMessage("Código de ativação inválido");
            }

            public async Task<bool> BeValidPair(Command command, string email, CancellationToken cancelation)
            {
                return await _context.Set<BaseUser>()
                    .AnyAsync(x => EF.Functions.Like(x.Email, command.Email) && x.ActivationCode == command.ActivationCode,
                        cancellationToken: cancelation);
            }
        }

        public class Handler : BaseCommandHandler<Command, DbContext>
        {
            private readonly IAuthenticationMailService _mailingService;

            public Handler(DbContext context, IHttpContextAccessor httpContextAccessor, IAuthenticationMailService mailingService) : base(context, httpContextAccessor)
            {
                _mailingService = mailingService;
            }

            protected override async Task<(Guid? entityId, object result)> ExecuteAsync(Command request)
            {
                var user = await _context.Set<BaseUser>()
                     .SingleAsync(x => EF.Functions.Like(x.Email, request.Email));

                user.Confirm();

                await _context.SaveChangesAsync();

                _mailingService.SendConfirmationSuccessMail(user.DisplayName, user.Email);

                return (null, null);
            }
        }
    }
}
