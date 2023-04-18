using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;

namespace Demo1.BusinessLogic.Commands.Folders
{
    public class CreateFolder
    {
        public class Request: IRequest<CommandResponse> //, IHasReadingModel<Models.Read.Folder>
        {
            
        }

        public class Validator: AbstractValidator<Request>
        {

        }

        public class Handler : IRequestHandler<Request, CommandResponse>
        {
            public Task<CommandResponse> Handle(Request request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
