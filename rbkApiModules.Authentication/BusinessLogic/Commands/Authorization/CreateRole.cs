using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.MediatR;
using rbkApiModules.Infrastructure.MediatR.SqlServer;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace rbkApiModules.Authentication
{
    /// <summary>
    /// Comando para criar uma nova regra de acesso
    /// </summary>
    public class CreateRole
    {
        public class Command : IRequest<CommandResponse>
        {
            public string Name { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly DbContext _context;

            public Validator(DbContext context)
            {
                _context = context;

                CascadeMode = CascadeMode.Stop;

                RuleFor(a => a.Name)
                    .IsRequired()
                    // FIXME: .MustHasLengthBetween(ModelConstants.Generic.Name.MinLength, ModelConstants.Generic.Name.MaxLength)
                    .MustAsync(NotExistOnDatabase).WithMessage("Já existe uma regra de acesso cadastrada com este nome.")
                    .WithName("Regra de Acesso");
            }

            /// <summary>
            /// Validador que verifica se o nome da regra já existe no banco de dados
            /// </summary>
            public async Task<bool> NotExistOnDatabase(Command command, string roleName, CancellationToken cancelation)
            {
                var query = _context.Set<Role>().Select(x => new { x.Name });

                return !await query.AnyAsync(x => EF.Functions.Like(x.Name, roleName));
            }
        }

        public class Handler : BaseCommandHandler<Command, DbContext>
        {
            public Handler(DbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
            {
            }

            protected override async Task<(Guid? entityId, object result)> ExecuteAsync(Command request)
            {
                var role = new Role(request.Name);

                await _context.Set<Role>().AddAsync(role);

                await _context.SaveChangesAsync();

                return (role.Id, role);
            }
        }
    }
}