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
    /// Comando para adicionar um claim a um usuário existente
    /// </summary>
    public class AddClaimToUser
    {
        public class Command : IRequest<CommandResponse>
        {
            public string Username { get; set; }
            public Guid ClaimId { get; set; }
            public ClaimAcessType AccessType { get; set; }
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
                    // FIXME: .MustHasLengthBetween(ModelConstants.Authentication.Username.MinLength, ModelConstants.Authentication.Username.MaxLength)
                    .MustAsync(UserExistOnDatabase).WithMessage("Usuário não encontrado no banco de dados.")
                    .WithName("Usuário");

                RuleFor(a => a.ClaimId)
                    .MustExistInDatabase<Command, Claim>(context)
                    .MustAsync(ClaimIsNotAssociatedWithUser).WithMessage("O controle de acesso já está associado a esse usuário.")
                    .WithName("Controle de Acesso");
            }

            /// <summary>
            /// Validador que verifica se o usuário existe no banco
            /// </summary>
            public async Task<bool> UserExistOnDatabase(Command command, string username, CancellationToken cancelation)
            {
                return await _context.Set<BaseUser>().AnyAsync(x => EF.Functions.Like(x.Username, username));
            }

            /// <summary>
            /// Validador que verifica se o controle de acesso já não está associado à regra de acesso
            /// </summary>
            public async Task<bool> ClaimIsNotAssociatedWithUser(Command command, Guid id, CancellationToken cancelation)
            {
                var user = await _context.Set<BaseUser>().SingleAsync(x => EF.Functions.Like(x.Username, command.Username));

                return !await _context.Set<UserToClaim>()
                    .AnyAsync(x => x.UserId == user.Id && x.ClaimId == command.ClaimId);
            }
        }

        /// <summary>
        /// Lógica de associação de um claim a um usuário
        /// </summary>
        public class Handler : BaseCommandHandler<Command, DbContext>
        {
            public Handler(DbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
            {
            }

            protected override async Task<(Guid? entityId, object result)> ExecuteAsync(Command request)
            {
                var user = await _context.Set<BaseUser>().Include(x => x.Claims)
                    .SingleAsync(x => EF.Functions.Like(x.Username, request.Username));

                var claim = await _context.Set<Claim>()
                    .SingleAsync(x => x.Id == request.ClaimId);

                user.AddClaim(claim, request.AccessType);

                await _context.SaveChangesAsync();

                return (null, null);
            }
        }
    }
}