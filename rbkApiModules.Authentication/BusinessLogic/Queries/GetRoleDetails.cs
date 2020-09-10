using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.MediatR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace rbkApiModules.Authentication
{
    /// <summary>
    /// Comando para pegar os detalhes de uma regra de acesso
    /// </summary>
    public class GetRoleDetails
    {
        public class Command : IRequest<QueryResponse>
        {
            public Guid Id { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator(DbContext context)
            {
                CascadeMode = CascadeMode.Stop;

                RuleFor(x => x.Id)
                    .MustExistInDatabase<Command, Role>(context);
            }
        }
        public class Handler : BaseQueryHandler<Command, DbContext>
        {
            private readonly IMapper _mapper;

            public Handler(DbContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper) 
                : base(context, httpContextAccessor)
            {
                _mapper = mapper;
            }

            protected override async Task<object> ExecuteAsync(Command request)
            {
                var results = await _context.Set<Role>()
                    .Where(x => x.Id == request.Id)
                    .ProjectTo<Roles.Details>(_mapper.ConfigurationProvider)
                    .SingleAsync();

                return results;
            }
        }
    }
}
