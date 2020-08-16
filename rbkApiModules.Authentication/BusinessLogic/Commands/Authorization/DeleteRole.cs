using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure;
using System;
using System.Threading.Tasks;

namespace rbkApiModules.Authentication
{
    /// <summary>
    /// Comando para apagar uma regra de acesso 
    /// </summary>
    public class DeleteRole
    {
        public class Command : IRequest<CommandResponse>
        {
            public Guid RoleId { get; set; }
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
                    .SingleAsync(x => x.Id == request.RoleId);

                _context.Set<Role>().Remove(role);

                await _context.SaveChangesAsync();

                return (role.Id, role);
            }
        }
    }
}