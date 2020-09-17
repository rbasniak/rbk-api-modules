using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace rbkApiModules.Logging.Core
{
    public class DatabaseLogIntgerceptor : DbCommandInterceptor
    {
        //private Exception WrapEntityFrameworkException(DbCommand command, Exception ex)
        //{
        //    var newException = new Exception("EntityFramework command failed!", ex);
        //    AddParamsToException(command.Parameters, newException);
        //    return newException;
        //}

        //private void AddParamsToException(DbParameterCollection parameters, Exception exception)
        //{
        //    foreach (DbParameter param in parameters)
        //    {
        //        exception.Data.Add(param.ParameterName, param.Value.ToString());
        //    }
        //}

        public DatabaseLogIntgerceptor()
        {
        }

        public override void CommandFailed(DbCommand command, CommandErrorEventData eventData)
        {
            var parameters = command.Parameters;
            var exception = eventData.Exception;
            var type = eventData.Command.CommandType;
            
            Log.Error(exception, "Erro ao executar comando SQL", new [] { "Objeto1", "Objeto2", "Objeto3" });
        }

        public override Task CommandFailedAsync(DbCommand command, CommandErrorEventData eventData, CancellationToken cancellationToken = default)
        {
            var parameters = command.Parameters;
            var exception = eventData.Exception;
            var type = eventData.Command.CommandType;
            return base.CommandFailedAsync(command, eventData, cancellationToken);
        } 
    }
}
