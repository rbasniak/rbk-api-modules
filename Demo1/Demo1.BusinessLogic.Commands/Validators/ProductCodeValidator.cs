using FluentValidation;

namespace Demo1.BusinessLogic.Commands;

public interface IProductCode
{
    string ProductCode { get; }
}

public class ProductCodeValidator: AbstractValidator<IProductCode>
{
	public ProductCodeValidator()
	{
        RuleFor(x => x.ProductCode)
            .NotEmpty().WithMessage("{PropertyName} is required")
            .MinimumLength(3).WithMessage("{PropertyName} must have 3 characters")
            .MaximumLength(3).WithMessage("{PropertyName} must have 3 characters");
    }
}
