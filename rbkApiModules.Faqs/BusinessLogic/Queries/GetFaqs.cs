using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.MediatR.SqlServer;
using System;
using System.Threading.Tasks;
using rbkApiModules.Infrastructure.MediatR.Core;
using AutoMapper;
using System.Linq;

namespace rbkApiModules.Faqs
{
    public class GetFaqs
    {
        public class Command : IRequest<QueryResponse>
        {
            public string Tag { get; set; }
        } 

        public class Handler : BaseQueryHandler<Command, DbContext>
        {
            private readonly IMapper _mapper;

            public Handler(DbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
                : base(context, httpContextAccessor)
            {
                _mapper = mapper;
            }

            protected override async Task<object> ExecuteAsync(Command request)
            {
                var faqs = await _context.Set<Faq>().Where(x => x.Tag.ToLower() == request.Tag.ToLower()).ToListAsync();
                
                return _mapper.Map<FaqDetails[]>(faqs);
            }
        }
    }
}
