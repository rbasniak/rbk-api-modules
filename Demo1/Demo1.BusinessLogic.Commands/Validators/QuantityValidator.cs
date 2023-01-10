using FluentValidation;

namespace Demo1.BusinessLogic.Commands;

public interface IQuantity
{
    int Quantity { get; }
}

public class QuantityValidator: AbstractValidator<IQuantity>
{
	public QuantityValidator()
	{
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("{PropertyName} must be greater than zero");
    }
}
