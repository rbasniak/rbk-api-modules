using Microsoft.AspNetCore.Http;
using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Infrastructure.MediatR.Core
{
    public interface ICommonDatabaseValidations
    {
        ValidationResult[] ValidateExistingDbElements(IHttpContextAccessor httpContextAccessor, object command);
    }
}
