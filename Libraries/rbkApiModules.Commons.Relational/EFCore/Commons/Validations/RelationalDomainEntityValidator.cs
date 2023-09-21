using FluentValidation;  

namespace rbkApiModules.Commons.Relational;

public class RelationalDomainEntityValidator : AbstractValidator<object> // DO NOT RENAME THIS CLASS, it instantiated using reflection
{
    public RelationalDomainEntityValidator(object parentValidator)
    {
        RuleFor(x => x)
            .MustMatchAllDatabaseConstrains(parentValidator).WithMessage("none"); // DO NOT CHANGE THIS MESSAGE! It's used in the pipeline to idenfity these errors
    }
}