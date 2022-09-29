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
    /// Comando para proteger uma permissão de acesso 
    /// </summary>
    public class ProtectClaim
    {
        public class Command : IRequest<CommandResponse>
        {
            public Guid Id { get; set; }
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
                    .MustExistInDatabase<Command, Claim>(context)
                    .WithMessage("Acesso não existe no servidor.")
                    .WithName("Permissão de Acesso"); 
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
                    .FindAsync(request.Id);

                claim.Protect();

                await _context.SaveChangesAsync();

                return (claim.Id, claim);
            }
        }
    }
}