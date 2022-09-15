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
    public class UpdateUserRoles
    {
        public class Command : IRequest<CommandResponse>
        {
            public string Username { get; set; }
            public Guid[] RolesIds { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly DbContext _context;

            public Validator(DbContext context)
            {
                _context = context;

                CascadeMode = CascadeMode.Stop;

                RuleFor(a => a.Username)
                    .IsRequired()
                    .MustAsync(UserExistOnDatabase).WithMessage("Usuário não encontrado no banco de dados.")
                    .WithName("Usuário");

                RuleForEach(a => a.RolesIds)
                    .MustAsync(RoleExistInDatabase).WithMessage("Não foi possível localizar o role no servidor")
                    .WithName("Controle de Acesso");
;
            }

            /// <summary>
            /// Validador que verifica se o usuário existe no banco
            /// </summary>
            public async Task<bool> UserExistOnDatabase(Command command, string username, CancellationToken cancelation)
            {
                return await _context.Set<BaseUser>().AnyAsync(x => EF.Functions.Like(x.Username, username));
            }

            private async Task<bool> RoleExistInDatabase(Command command, Guid id, CancellationToken cancelation)
            {
                return await _context.Set<Role>().AnyAsync(x => x.Id == id);
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
                    .Include(x => x.Claims).ThenInclude(x => x.Claim)
                    .Include(x => x.Roles).ThenInclude(x => x.Role).ThenInclude(x => x.Claims).ThenInclude(x => x.Claim)
                    .SingleAsync(x => EF.Functions.Like(x.Username, request.Username));

                _context.RemoveRange(user.Roles);

                foreach (var roleId in request.RolesIds)
                {
                    var role = await _context.Set<Role>().SingleAsync(c => c.Id == roleId);
                    user.AddRole(role);
                }

                await _context.SaveChangesAsync();

                return (null, user.Roles);
            }
        }
    }
}