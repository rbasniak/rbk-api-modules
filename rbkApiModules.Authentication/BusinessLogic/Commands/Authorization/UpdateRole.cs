using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.MediatR.SqlServer;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using rbkApiModules.Infrastructure.MediatR.Core;

namespace rbkApiModules.Authentication
{
    public class UpdateRole
    {
        public class Command : IRequest<CommandResponse>
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly DbContext _context;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Validator(DbContext context, IHttpContextAccessor httpContextAccessor)
            {
                _httpContextAccessor = httpContextAccessor;
                _context = context;

                CascadeMode = CascadeMode.Stop;

                RuleFor(a => a.Id)
                    .MustExistInDatabase<Command, Role>(context)
                    .MustAsync(HaveSameUserAuthGroup).WithMessage("Acesso negado.")
                    .MustAsync(NotExistOnDatabaseInSameGroup).WithMessage("Já existe uma regra de acesso cadastrada com este nome.")
                    .WithName("Regra de Acesso"); 
            }

            private async Task<bool> HaveSameUserAuthGroup(Command command, Guid id, CancellationToken cancelation)
            {
                var authenticationGroup = _httpContextAccessor.GetAuthenticationGroup();
                var role = await _context.Set<Role>().FindAsync(id);
                return role.AuthenticationGroup == authenticationGroup;
            }

            public async Task<bool> NotExistOnDatabaseInSameGroup(Command command, Guid id, CancellationToken cancelation)
            {
                var authenticationGroup = _httpContextAccessor.GetAuthenticationGroup();

                return !await _context.Set<Role>()
                    .AnyAsync(role => 
                        role.Id != command.Id &&
                        EF.Functions.Like(role.Name, command.Name) && 
                        EF.Functions.Like(role.AuthenticationGroup, authenticationGroup));
            }
        }

        public class Handler : BaseCommandHandler<Command, DbContext>
        {
            public Handler(DbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
            {
            }

            protected override async Task<(Guid? entityId, object result)> ExecuteAsync(Command request)
            {
                var role = new Role(request.Name, _httpContextAccessor.GetAuthenticationGroup());

                await _context.AddAsync(role);

                await _context.SaveChangesAsync();

                return (role.Id, role);
            }
        }
    }
}