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
    /// Comando para criar uma nova permissão de acesso
    /// </summary>
    public class CreateClaim
    {
        public class Command : IRequest<CommandResponse>
        {
            public string Name { get; set; }
            public string Description { get; set; }
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

                RuleFor(a => a.Name)
                    .IsRequired()
                    .MustAsync(NotExistOnDatabaseInSameGroup).WithMessage("Já existe uma permissão de acesso cadastrada com este nome.")
                    .WithName("Permissão de Acesso");
            }

            public async Task<bool> NotExistOnDatabaseInSameGroup(Command command, string name, CancellationToken cancelation)
            {
                var authenticationGroup = _httpContextAccessor.GetAuthenticationGroup();

                return !await _context.Set<Claim>()
                    .AnyAsync(role => 
                        EF.Functions.Like(role.Name, name) && 
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
                var claim = new Claim(request.Name, request.Description, _httpContextAccessor.GetAuthenticationGroup());

                await _context.AddAsync(claim);

                await _context.SaveChangesAsync();

                return (claim.Id, claim);
            }
        }
    }
}