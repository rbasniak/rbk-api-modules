using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.MediatR.Core;
using rbkApiModules.Infrastructure.MediatR.SqlServer;
using rbkApiModules.Workflow.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Workflow
{
    public class GetStatesDotCode
    {
        public class Command : IRequest<QueryResponse>
        {
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
            }
        }

        public class Handler : BaseQueryHandler<Command, DbContext>
        {
            private readonly IMapper _mapper;

            public Handler(DbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
            {
                _mapper = mapper;
            }

            protected override async Task<object> ExecuteAsync(Command request)
            {
                var states = await _context.Set<State>()
                    .Include(x => x.Transitions).ThenInclude(x => x.Event)
                    .Include(x => x.Transitions).ThenInclude(x => x.FromState)
                    .Include(x => x.Transitions).ThenInclude(x => x.ToState)
                    .Include(x => x.UsedBy).ThenInclude(x => x.Event)
                    .Include(x => x.UsedBy).ThenInclude(x => x.FromState)
                    .Include(x => x.UsedBy).ThenInclude(x => x.ToState)
                    .ToListAsync();

                DotCodeGenerator.GenerateCode(states);

                var transitions = await _context.Set<Transition>()
                    .Include(x => x.ToState)
                    .Include(x => x.FromState).ToListAsync();

                return new Result()
                {
                    Code = DotCodeGenerator.GenerateCode(states)
                };
            }
        }

        public class Result
        {
            public string Code { get; set; }
        }
    }
}
