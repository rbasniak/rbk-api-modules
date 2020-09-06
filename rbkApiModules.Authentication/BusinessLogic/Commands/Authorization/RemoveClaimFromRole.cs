using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure;
using rbkApiModules.Infrastructure.MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace rbkApiModules.Authentication
{
    /// <summary>
    /// Comando para remover um claim a uma regra de acesso existente
    /// </summary>
    public class RemoveClaimFromRole
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
                    .MustAsync(ClaimIsAssociatedWithRole).WithMessage("O controle de acesso não pertence à regra de acesso.")
                    .WithName("Controle de Acesso");
            }

            /// <summary>
            /// Validador que verifica se o controle de acesso está associado à regra de acesso para ter o que remover
            /// </summary>
            public async Task<bool> ClaimIsAssociatedWithRole(Command command, Guid id, CancellationToken cancelation)
            {
                return await _context.Set<RoleToClaim>()
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
                var entity = await _context.Set<RoleToClaim>()
                    .SingleAsync(x => x.RoleId == request.RoleId && x.ClaimId == request.ClaimId);

                _context.Set<RoleToClaim>().Remove(entity);

                await _context.SaveChangesAsync();

                return (null, null);
            }
        }
    }
}