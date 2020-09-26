using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace rbkApiModules.Analytics.Core
{
    public class DatabaseLogInterceptor : DbCommandInterceptor
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

        private readonly ITransactionCounter _transactionCounter;

        public DatabaseLogInterceptor(ITransactionCounter transactionCounter)
        {
            _transactionCounter = transactionCounter;
        }

        private Exception WrapEntityFrameworkException(DbCommand command, Exception ex)
        {
            var newException = new Exception("EFCore command failed", ex);
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

        public override DbCommand CommandCreated(CommandEndEventData eventData, DbCommand result)
        {
            _transactionCounter.TotalTime += eventData.Duration.TotalMilliseconds;

            return base.CommandCreated(eventData, result);
        }

        public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
        {
            _transactionCounter.TotalTime += eventData.Duration.TotalMilliseconds;
            _transactionCounter.Transactions++;

            return base.NonQueryExecuted(command, eventData, result);
        }

        public override Task<int> NonQueryExecutedAsync(DbCommand command, CommandExecutedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            _transactionCounter.TotalTime += eventData.Duration.TotalMilliseconds;
            _transactionCounter.Transactions++;

            return base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
        }

        public override object ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, object result)
        {
            _transactionCounter.TotalTime += eventData.Duration.TotalMilliseconds;
            _transactionCounter.Transactions++;

            return base.ScalarExecuted(command, eventData, result);
        }

        public override Task<object> ScalarExecutedAsync(DbCommand command, CommandExecutedEventData eventData, object result, CancellationToken cancellationToken = default)
        {
            _transactionCounter.TotalTime += eventData.Duration.TotalMilliseconds;
            _transactionCounter.Transactions++;

            return base.ScalarExecutedAsync(command, eventData, result, cancellationToken);
        }

        public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
        {
            _transactionCounter.TotalTime += eventData.Duration.TotalMilliseconds;
            _transactionCounter.Transactions++;

            return base.ReaderExecuted(command, eventData, result);
        }

        public override Task<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
        {
            _transactionCounter.TotalTime += eventData.Duration.TotalMilliseconds;
            _transactionCounter.Transactions++;

            return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
        }
         
        public override void CommandFailed(DbCommand command, CommandErrorEventData eventData)
        {
            var parameters = command.Parameters;
            var exception = eventData.Exception;
            var type = eventData.Command.CommandType;
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
