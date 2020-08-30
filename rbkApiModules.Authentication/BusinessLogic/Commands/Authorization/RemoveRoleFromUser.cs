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
    /// Comando para remover uma regra de acesso de um usuário existente
    /// </summary>
    public class RemoveRoleFromUser
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
                    .MustAsync(RoleIsAssociatedWithUser).WithMessage("A regra de acesso não está associado a esse usuário.")
                    .WithName("Regra de Acesso");
            }

            /// <summary>
            /// Validador que verifica se o usuário existe no banco
            /// </summary>
            public async Task<bool> UserExistOnDatabase(Command command, string username, CancellationToken cancelation)
            {
                return await _context.Set<BaseUser>().AnyAsync(x => EF.Functions.Like(x.Username, username));
            }

            /// <summary>
            /// Validador que verifica se a regra de acesso está associado ao usuário para ter o que remover
            /// </summary>
            public async Task<bool> RoleIsAssociatedWithUser(Command command, Guid id, CancellationToken cancelation)
            {
                var user = await _context.Set<BaseUser>().SingleAsync(x => EF.Functions.Like(x.Username, command.Username));

                return await _context.Set<UserToRole>()
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
                var user = await _context.Set<BaseUser>().SingleAsync(x => EF.Functions.Like(x.Username, request.Username));

                var entity = await _context.Set<UserToRole>()
                    .SingleAsync(x => x.UserId == user.Id && x.RoleId == request.RoleId);

                _context.Set<UserToRole>().Remove(entity);

                await _context.SaveChangesAsync();

                return (null, null);
            }
        }
    }
}