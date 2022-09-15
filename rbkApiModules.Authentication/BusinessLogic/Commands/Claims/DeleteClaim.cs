using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.MediatR.SqlServer;
using System;
using System.Threading.Tasks;
using rbkApiModules.Infrastructure.MediatR.Core;
using System.Threading;
using System.Linq;

namespace rbkApiModules.Authentication
{
    /// <summary>
    /// Comando para apagar uma permissão de acesso 
    /// </summary>
    public class DeleteClaim
    {
        public class Command : IRequest<CommandResponse>
        {
            public Guid ClaimId { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly DbContext _context;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Validator(DbContext context, IHttpContextAccessor httpContextAccessor)
            {
                _context = context;
                _httpContextAccessor = httpContextAccessor;

                CascadeMode = CascadeMode.Stop;

                RuleFor(a => a.ClaimId)
                    .MustExistInDatabase<Command, Claim>(context)
                    .MustAsync(MustBeFromSameAuthenticationGroup).WithMessage("Security breach, claim from another authentication group")
                    .MustAsync(MustNotBeUsedInAnyRole).WithMessage("Acesso está sendo utilizado em uma ou mais permissões")
                    .MustAsync(MustNotBeUsedInAnyUser).WithMessage("Acesso está sendo utilizado em um ou mais usuários")
                    .WithName("Regra de Acesso");
            }

            private async Task<bool> MustBeFromSameAuthenticationGroup(Command command, Guid id, CancellationToken cancellation)
            {
                var claim = await _context.Set<Claim>().FindAsync(id);
                var group = _httpContextAccessor.GetAuthenticationGroup();
                var userAuthenticationGroup = _httpContextAccessor.GetAuthenticationGroup();

                return claim.AuthenticationGroup == group && userAuthenticationGroup == group;
            }

            private async Task<bool> MustNotBeUsedInAnyRole(Command command, Guid id, CancellationToken cancellation)
            {
                var claim = await _context.Set<Claim>().FindAsync(id);
                var authenticationGroup = _httpContextAccessor.GetAuthenticationGroup();

                return !await _context.Set<Role>()
                    .Include(x => x.Claims).ThenInclude(x => x.Claim)
                    .AnyAsync(role =>
                        role.Claims.Any(x => x.ClaimId == id) &&
                        EF.Functions.Like(role.AuthenticationGroup, authenticationGroup));
            }

            private async Task<bool> MustNotBeUsedInAnyUser(Command command, Guid id, CancellationToken cancellation)
            {
                var claim = await _context.Set<Claim>().FindAsync(id);
                var authenticationGroup = _httpContextAccessor.GetAuthenticationGroup();

                return !await _context.Set<BaseUser>()
                    .Include(x => x.Claims).ThenInclude(x => x.Claim)
                    .AnyAsync(user =>
                        user.Claims.Any(x => x.ClaimId == id) &&
                        EF.Functions.Like(user.AuthenticationGroup, authenticationGroup));
            }
        }

        public class Handler : BaseCommandHandler<Command, DbContext>
        {
            public Handler(DbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
            {
            }

            protected override async Task<(Guid? entityId, object result)> ExecuteAsync(Command request)
            {
                var claim = await _context.Set<Claim>()
                    .SingleAsync(x => x.Id == request.ClaimId);

                _context.Set<Claim>().Remove(claim);

                await _context.SaveChangesAsync();

                return (claim.Id, claim);
            }
        }
    }
}