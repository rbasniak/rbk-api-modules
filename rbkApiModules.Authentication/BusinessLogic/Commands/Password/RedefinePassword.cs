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
    public class RedefinePassword
    {
        public class Command : IRequest<CommandResponse>
        {
            public string Code { get; set; }
            public string Password { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly DbContext _context;

            public Validator(DbContext context)
            {
                _context = context;

                CascadeMode = CascadeMode.Stop;

                RuleFor(a => a.Code)
                     .IsRequired()
                     .MustAsync(ExistOnDatabaseAndIsValid).WithMessage("Código de redefiniçãode senha expirado ou já utilizado.")
                     .WithName("Código de Redefinição de Senha");

                RuleFor(a => a.Password)
                     .IsRequired()
                     .WithName("Senha");
            }

            public async Task<bool> ExistOnDatabaseAndIsValid(Command command, string code, CancellationToken cancelation)
            {
                var user = await _context.Set<BaseUser>()
                    .Include(x => x.PasswordRedefineCode)
                    .SingleOrDefaultAsync(x => EF.Functions.Like(x.PasswordRedefineCode.Hash, code), cancellationToken: cancelation);

                return user != null
                    && user.PasswordRedefineCode.CreationDate.HasValue
                    && (DateTime.UtcNow - user.PasswordRedefineCode.CreationDate).Value.TotalHours >= 24;
            }
        }

        public class Handler : BaseCommandHandler<Command, DbContext>
        {
            public Handler(DbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
            {
            }

            protected override async Task<(Guid? entityId, object result)> ExecuteAsync(Command request)
            {
                var user = await _context.Set<BaseUser>()
                    .SingleAsync(x => EF.Functions.Like(x.PasswordRedefineCode.Hash, request.Code));

                user.SetPassword(request.Password);
                user.UsePasswordRedefineCode();

                await _context.SaveChangesAsync();

                return (null, null);
            }
        }
    }
}
