using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace rbkApiModules.Commons.Core;

public interface ICommonDatabaseValidations
{
    ValidationResult[] ValidateExistingDbElements(IHttpContextAccessor httpContextAccessor, object command);

    ValidationResult[] ValidateIsUniqueDbElements(IHttpContextAccessor httpContextAccessor, object command);

    ValidationResult[] ValidateNonUsedDbElements(IHttpContextAccessor httpContextAccessor, object command);
}