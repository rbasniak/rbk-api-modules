using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.CQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo1.BusinessLogic.Commands.Folders
{
    public class CreateFolder
    {
        public class Command: IRequest<CommandResponse> //, IHasReadingModel<Models.Read.Folder>
        {
            
        }

        public class Validator: AbstractValidator<Command>
        {

        }

        public class Handler : IRequestHandler<Command, CommandResponse>
        {
            public Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
