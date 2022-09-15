using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.MediatR.SqlServer;
using System;
using System.Threading;
using System.Threading.Tasks;
using rbkApiModules.Infrastructure.MediatR.Core;
using System.Linq;

namespace rbkApiModules.Authentication
{
    /// <summary>
    /// Comando para adicionar um claim a uma regra de acesso existente
    /// </summary>
    public class UpdateRoleClaims
    {
        public class Command : IRequest<CommandResponse>
        {
            public Guid Id { get; set; }
            public Guid[] ClaimsIds { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly DbContext _context;

            public Validator(DbContext context)
            {
                _context = context;

                CascadeMode = CascadeMode.Stop;

                RuleFor(a => a.Id)
                    .MustExistInDatabase<Command, Role>(context)
                    .WithName("Regra de Acesso");

                RuleForEach(a => a.ClaimsIds)
                    .MustAsync(ClaimExistInDatabase).WithMessage("Não foi possível localizar o claim no servidor")
                    .WithName("Controle de Acesso");
;
            }

            private async Task<bool> ClaimExistInDatabase(Command command, Guid id, CancellationToken cancelation)
            {
                return await _context.Set<Claim>().AnyAsync(x => x.Id == id);
            }
        }

        public class Handler : BaseCommandHandler<Command, DbContext>
        {
            public Handler(DbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
            {
            }

            protected override async Task<(Guid? entityId, object result)> ExecuteAsync(Command request)
            {
                var role = await _context.Set<Role>()
                    .Include(x => x.Claims)
                    .SingleAsync(x => x.Id == request.Id);

                _context.RemoveRange(role.Claims);

                foreach (var claimId in request.ClaimsIds)
                {
                    var claim = await _context.Set<Claim>().SingleAsync(c => c.Id == claimId);
                    role.AddClaim(claim);
                }

                await _context.SaveChangesAsync();

                return (role.Id, role);
            }
        }
    }
}