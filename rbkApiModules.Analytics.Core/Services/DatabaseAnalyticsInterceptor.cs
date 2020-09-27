using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace rbkApiModules.Analytics.Core
{
    public class DatabaseAnalyticsInterceptor : DbCommandInterceptor
    { 
        private readonly ITransactionCounter _transactionCounter;

        public DatabaseAnalyticsInterceptor(ITransactionCounter transactionCounter)
        {
            _transactionCounter = transactionCounter;
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
    }
}
