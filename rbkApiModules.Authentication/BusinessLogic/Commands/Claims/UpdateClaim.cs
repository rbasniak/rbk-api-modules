﻿using FluentValidation;
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
    /// Comando para atualizar uma permissão de acesso 
    /// </summary>
    public class UpdateClaim
    {
        public class Command : IRequest<CommandResponse>
        {
            public Guid Id { get; set; }
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

                RuleFor(a => a.Id)
                    .MustExistInDatabase<Command, Claim>(context)
                    .MustAsync(HaveSameUserAuthGroup).WithMessage("Acesso negado.")
                    .WithName("Permissão de Acesso"); 
            }

            private async Task<bool> HaveSameUserAuthGroup(Command command, Guid id, CancellationToken cancelation)
            {
                var authenticationGroup = _httpContextAccessor.GetAuthenticationGroup();
                var claim = await _context.Set<Claim>().FindAsync(id);
                return claim.AuthenticationGroup == authenticationGroup;
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

                claim.SetDescription(request.Description);

                await _context.SaveChangesAsync();

                return (claim.Id, claim);
            }
        }
    }
}