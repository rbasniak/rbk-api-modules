﻿using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Demo.Database;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.MediatR.SqlServer;
using rbkApiModules.Infrastructure.MediatR.Core;
using AutoMapper;
using rbkApiModules.Infrastructure.Models;

namespace rbkApiModules.Demo.BusinessLogic
{
    public class GetAllDemoUsers
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

        public class Handler : BaseQueryHandler<Command, DatabaseContext>
        {
            public Handler(DatabaseContext context, IHttpContextAccessor httpContextAccessor)
                : base(context, httpContextAccessor)
            {
            }

            protected async override Task<object> ExecuteAsync(Command request)
            {
                var users =  await _context.Users.ToListAsync();

                return users;
            }
        }
    }
}
