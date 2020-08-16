using AspNetCoreApiTemplate.Utilities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace rbkApiModules.Authentication
{
    /// <summary>
    /// Comando para adicionar uma regra de aceso a um usuário existente
    /// </summary>
    public class AddRoleToUser
    {
        public class Command : IRequest<CommandResponse>
        {
            public string Username { get; set; }
            public Guid RoleId { get; set; }
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

                RuleFor(a => a.RoleId)
                    .MustExistInDatabase<Command, Role>(context)
                    .MustAsync(RoleIsNotAssociatedWithUser).WithMessage("A regra de acesso já está associado a esse usuário.")
                    .WithName("Regra de Acesso");
            }

            /// <summary>
            /// Validador que verifica se o usuário existe no banco
            /// </summary>
            public async Task<bool> UserExistOnDatabase(Command command, string username, CancellationToken cancelation)
            {
                return await _context.Set<User>().AnyAsync(x => EF.Functions.Like(x.Username, username));
            }

            /// <summary>
            /// Validador que verifica se a regra de acesso já não está associado ao usuário
            /// </summary>
            public async Task<bool> RoleIsNotAssociatedWithUser(Command command, Guid id, CancellationToken cancelation)
            {
                var user = await _context.Set<User>().SingleAsync(x => EF.Functions.Like(x.Username, command.Username));

                return !await _context.Set<UserToRole>()
                    .AnyAsync(x => x.UserId == user.Id && x.RoleId == command.RoleId);
            }
        }

        public class Handler : BaseCommandHandler<Command, DbContext>
        {
            public Handler(DbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
            {
            }

            protected override async Task<(Guid? entityId, object result)> ExecuteAsync(Command request)
            {
                var user = await _context.Set<User>()
                    .Include(x => x.Roles)
                        .SingleAsync(x => EF.Functions.Like(x.Username, request.Username));

                var role = await _context.Set<Role>()
                    .SingleAsync(x => x.Id == request.RoleId);

                user.AddRole(role);

                await _context.SaveChangesAsync();

                return (null, null);
            }
        }
    }
}