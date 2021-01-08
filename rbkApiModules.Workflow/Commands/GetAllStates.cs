using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.MediatR.Core;
using rbkApiModules.Infrastructure.MediatR.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Workflow
{
    public class GetAllStates
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
                var transitions = await _context.Set<Transition>().ToListAsync();
                var groups = await _context.Set<StateGroup>().ToListAsync();
                var events = await _context.Set<Event>().ToListAsync();
                var states = await _context.Set<State>().ToListAsync();
                var claims = await _context.Set<ClaimToEvent>().ToListAsync();

                var result = _mapper.Map<StateGroupDetails[]>(groups);

                return result;
            }
        }
    }
}
