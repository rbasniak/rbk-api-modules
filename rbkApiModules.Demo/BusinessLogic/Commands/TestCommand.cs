using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Demo.Database;
using rbkApiModules.Demo.Models;
using System;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.MediatR.SqlServer;
using rbkApiModules.Infrastructure.MediatR.Core;

namespace rbkApiModules.Demo.BusinessLogic
{
    public class TestCommand
    {
        public class Command : IRequest<CommandResponse>, ITestExternalValidator
        {
            public string Data1 { get; set; }
            public string Data2 { get; set; }
            public string Data3 { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly DatabaseContext _context;

            public Validator(DatabaseContext context)
            {
                _context = context;

                RuleFor(x => x.Data1)
                    .Must(MustTest).WithMessage("Testeeee");

                RuleFor(x => x.Data3)
                    .Must(MustTest2).WithMessage("xxxxxxxxxxxxxxxx");
            }

            private bool MustTest(Command command, string test)
            {
                return false;
            }

            private bool MustTest2(Command command, string test)
            {
                return true;
            }
        }

        public class Handler : BaseCommandHandler<Command, DatabaseContext>
        {
            public Handler(DatabaseContext context, IHttpContextAccessor httpContextAccessor)
                : base(context, httpContextAccessor)
            {
            }

            protected override async Task<(Guid? entityId, object result)> ExecuteAsync(Command request)
            {
                 

                return (null, null);
            }
        }
    }

    public interface ITestExternalValidator
    {
        public string Data2 { get; set; }
    }

    public class PlantDomainValidator : AbstractValidator<ITestExternalValidator>
    {
        private readonly DatabaseContext _context;
        public PlantDomainValidator(DatabaseContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;

            RuleFor(x => x.Data2)
                .IsRequired();
        }
    }
}
