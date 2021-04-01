using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.MediatR.Core;
using rbkApiModules.Infrastructure.MediatR.SqlServer;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace rbkApiModules.Payment.SqlServer
{
    public class ActivateTrial
    {
        public class Command : IRequest<CommandResponse>
        { 
            public Guid Key { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly DbContext _context;

            public Validator(DbContext context)
            {
                _context = context;

                CascadeMode = CascadeMode.Stop;

                RuleFor(x => x.Key)
                    .MustExistInDatabase<Command, TrialKey>(context).WithName("Chave de teste")
                    .MustAsync(KeyIsUnused).WithMessage("Chave de teste já em uso")
                    .MustAsync(NotHaveActivePlan).WithMessage("Chave de teste não possui um plano ativo");

            }

            private async Task<bool> KeyIsUnused(Command command, Guid keyId, CancellationToken cancelation)
            {
                var key = await _context.Set<TrialKey>().FindAsync(new object[] { keyId }, cancellationToken: cancelation);
                return key.IsAvailable;
            }

            private async Task<bool> NotHaveActivePlan(Command command, Guid keyId, CancellationToken cancelation)
            {
                var client = await _context.Set<BaseClient>()
                    .Include(x => x.Plan)
                    .SingleAsync(x => x.TrialKeyId == keyId, cancellationToken: cancelation);

                return client.Plan.IsDefault;
            }
        }

        public class Handler : BaseCommandHandler<Command, DbContext>
        {
            private readonly IMapper _mapper;

            public Handler(DbContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper) : base(context, httpContextAccessor)
            {
                _mapper = mapper;
            }

            protected override async Task<(Guid? entityId, object result)> ExecuteAsync(Command request)
            {
                var client = await _context.Set<BaseClient>()
                    .Include(x => x.Plan)
                    .Include(x => x.TrialKey)
                        .ThenInclude(x => x.Plan)
                    .SingleAsync(x => x.TrialKeyId == request.Key);

                client.TrialKey.Activate();
                client.UseTrialPlan();

                await _context.SaveChangesAsync();

                return (null, _mapper.Map<TrialKeyDto.Details>(client.TrialKey));
            }
        }
    }
}
