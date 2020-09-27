using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Diagnostics;
using rbkApiModules.Diagnostics.Commons;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace rbkApiModules.Diagnostics.Core
{
    public class DatabaseDiagnosticsInterceptor : DbCommandInterceptor
    { 
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DatabaseDiagnosticsInterceptor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private Exception WrapEntityFrameworkException(DbCommand command, Exception ex)
        {
            var newException = new Exception("Database command failed", ex);
            AddParamsToException(command.Parameters, newException);
            return newException;
        }

        private void AddParamsToException(DbParameterCollection parameters, Exception exception)
        {
            foreach (DbParameter param in parameters)
            {
                exception.Data.Add(param.ParameterName, param.Value.ToString());
            }
        }

        private void AddExceptionToContext(DbCommand command, Exception exception)
        {
            var exceptionData = WrapEntityFrameworkException(command, exception);

            if (_httpContextAccessor.HttpContext.Items.TryGetValue("sql-exception", out object exceptions))
            {
                var list = exceptions as List<Exception>;
                list.Add(exceptionData);
            }
            else
            {
                _httpContextAccessor.HttpContext.Items.Add("sql-exception", new List<Exception> { exceptionData });
            }
        } 

        public override void CommandFailed(DbCommand command, CommandErrorEventData eventData)
        {
            AddExceptionToContext(command, eventData.Exception);
        }

        public override Task CommandFailedAsync(DbCommand command, CommandErrorEventData eventData, CancellationToken cancellationToken = default)
        {
            AddExceptionToContext(command, eventData.Exception);

            return base.CommandFailedAsync(command, eventData, cancellationToken);
        }
    }
}
