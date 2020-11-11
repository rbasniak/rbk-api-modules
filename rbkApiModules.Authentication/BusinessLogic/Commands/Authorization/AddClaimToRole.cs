using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.MediatR.SqlServer;
using System;
using System.Threading;
using System.Threading.Tasks;
using rbkApiModules.Infrastructure.MediatR.Core;

namespace rbkApiModules.Authentication
{
    /// <summary>
    /// Comando para adicionar um claim a uma regra de acesso existente
    /// </summary>
    public class AddClaimToRole
    {
        public class Command : IRequest<CommandResponse>
        {
            public Guid RoleId { get; set; }
            public Guid ClaimId { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly DbContext _context;

            public Validator(DbContext context)
            {
                _context = context;

                CascadeMode = CascadeMode.Stop;

                RuleFor(a => a.RoleId)
                    .MustExistInDatabase<Command, Role>(context)
                    .WithName("Regra de Acesso");

                RuleFor(a => a.ClaimId)
                    .MustExistInDatabase<Command, Claim>(context)
                    .MustAsync(ClaimIsNotAssociatedWithRole).WithMessage("O controle de acesso já pertence à regra de acesso.")
                    .WithName("Controle de Acesso");
            }

            /// <summary>
            /// Validador que verifica se o controle de acesso já não está associado à regra de acesso
            /// </summary>
            public async Task<bool> ClaimIsNotAssociatedWithRole(Command command, Guid id, CancellationToken cancelation)
            {
                return !await _context.Set<RoleToClaim>()
                    .AnyAsync(x => x.RoleId == command.RoleId && x.ClaimId == command.ClaimId);
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
                    .SingleAsync(x => x.Id == request.RoleId);

                var claim = await _context.Set<Claim>()
                    .SingleAsync(x => x.Id == request.ClaimId);

                role.AddClaim(claim);

                var temp = await _context.SaveChangesAsync();

                return (null, null);
            }
        }
    }
}