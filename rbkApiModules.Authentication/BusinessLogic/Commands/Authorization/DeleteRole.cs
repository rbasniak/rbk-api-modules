﻿using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.MediatR.SqlServer;
using System;
using System.Threading.Tasks;
using rbkApiModules.Infrastructure.MediatR.Core;
using System.Threading;

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
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Validator(DbContext context, IHttpContextAccessor httpContextAccessor)
            {
                _context = context;
                _httpContextAccessor = httpContextAccessor;

                CascadeMode = CascadeMode.Stop;

                RuleFor(a => a.RoleId)
                    .MustExistInDatabase<Command, Role>(context)
                    .MustAsync(MustBeFromSameAuthenticationGroup).WithMessage("Security breach, role from another authentication group")
                    .WithName("Regra de Acesso");
            }

            private async Task<bool> MustBeFromSameAuthenticationGroup(Command command, Guid id, CancellationToken cancellation)
            {
                var role = await _context.Set<Role>().FindAsync(id);
                var group = _httpContextAccessor.GetAuthenticationGroup();
                var userAuthenticationGroup = _httpContextAccessor.GetAuthenticationGroup();

                return role.AuthenticationGroup == group && userAuthenticationGroup == group;
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