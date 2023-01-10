using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;

namespace Demo1.BusinessLogic.Commands;

/// <summary>
/// 
/// SCENARIO:
/// This method demonstrates the validation pipeline
/// The idea is to validate everything first so the handler 
/// is 100% sure that it is working with valid data only.
/// This simplifies the code a lot, removing several if clauses
/// from the business logic code, making it more readable and
/// easier to maintain
/// 
/// NOTES:
/// - Common validators can be shared and abstracted through
///   interfaces (i.e. IQuantity and IProductCode)
/// - When the source of the command is a fronend client that
///   you trust it won't send some invalid data, some resource 
///   consuming validations could be ommited, like ckecking if
///   the entity exists in the database
/// 
/// POINTS OF ATTENTION: 
/// Database validations might not needed the full object 
/// loaded from database while the handler does. 
/// For really big objects or chunks of data this could have
/// a performance impact. There are two solutions:
/// 
/// 1) Load the entire object in the validator, then just get 
///    the object from memory in the handler.
///    PROS:
///      - Overall request speed is better
///      - Requires only 1 roundtrip to the database
///    CONS:
///      - Validator is slower in case it has a validation issue
///      - Requires the validator to be responsible for properly 
///        loading the entity.
///      - Handler unit test is a little trickier because it 
///        requires the entity already loaded in the DbContext
///    
/// 2) Load the object in the validator with the required
///    properties only and then fully load it on the handler
///    PROS:
///      - Logic of loading the full object hierarchy is
///        not hidden in the validator
///    CONS:
///      - Overall request speed is slightly worst
///      - Consumes more resources since it requires at 
///        least 2 roundtrips do the database 
///      - Small bits of duplicated code when the validator
///        requires the loading of some other properties 
///        as well
/// </summary>
public class PipelineValidationTest
{
    public class Command: IRequest<BaseResponse>, IProductCode, IQuantity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductCode { get; set; }
        public int Quantity { get; set; }
    }

    public class Validator: AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("{PropertyName} is required")
                .MinimumLength(3).WithMessage("{PropertyName} must have at least 3 characters")
                .MaximumLength(25).WithMessage("{PropertyName} must not have more than 25 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("{PropertyName} is required")
                .MinimumLength(10).WithMessage("{PropertyName} must have at least 10 characters")
                .MaximumLength(100).WithMessage("{PropertyName} must not have more than 100 characters");
        }
    }

    public class Handler : IRequestHandler<Command, BaseResponse>
    {
        public async Task<BaseResponse> Handle(Command request, CancellationToken cancellation)
        {
            return await Task.FromResult(new BaseResponse() { Result = new 
            { 
                ProductCode = request.ProductCode,
                Quantity = request.Quantity,
                Description = request.Description,
                Name = request.Name,
                Id = Guid.NewGuid()
            }});
        }
    }
}