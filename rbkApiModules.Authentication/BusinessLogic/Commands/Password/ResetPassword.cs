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
    public class ResetPassword
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
                    .MustAsync(EmailBeRegistered).WithMessage("E-mail não cadastrado");
            }

            private async Task<bool> EmailBeRegistered(Command command, string email, CancellationToken cancelation)
            {
                return await _context.Set<BaseUser>().AnyAsync(x => EF.Functions.Like(x.Email, email));
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

                user.SetPasswordRedefineCode(DateTime.Now);

                await _context.SaveChangesAsync();

                _mailingService.SendPasswordResetMail(user.DisplayName, user.Email, user.PasswordRedefineCode.Hash);

                return (null, null);
            }
        }
    }
}
