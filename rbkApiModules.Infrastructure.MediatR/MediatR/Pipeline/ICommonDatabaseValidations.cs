using Microsoft.AspNetCore.Http;
using rbkApiModules.Infrastructure.Models;

namespace rbkApiModules.Infrastructure.MediatR.Core
{
    public interface ICommonDatabaseValidations
    {
        ValidationResult[] ValidateExistingDbElements(IHttpContextAccessor httpContextAccessor, object command);

        ValidationResult[] ValidateIsUniqueDbElements(IHttpContextAccessor httpContextAccessor, object command);
        
        ValidationResult[] ValidateNonUsedDbElements(IHttpContextAccessor httpContextAccessor, object command);
    }
}
