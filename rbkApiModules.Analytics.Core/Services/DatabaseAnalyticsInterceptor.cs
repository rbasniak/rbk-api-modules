using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace rbkApiModules.Analytics.Core
{
    public class DatabaseAnalyticsInterceptor : DbCommandInterceptor
    { 
        public const string TRANSACTION_TIME_TOKEN = "transaction-count";
        public const string TRANSACTION_COUNT_TOKEN = "transaction-time";

        private readonly IHttpContextAccessor _httpContextAccessor;

        public DatabaseAnalyticsInterceptor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private void AddTransactionCount()
        {
            if (_httpContextAccessor.HttpContext == null) return;

            if (_httpContextAccessor.HttpContext.Items.TryGetValue(TRANSACTION_COUNT_TOKEN, out object rawCount))
            {
                var count = (int)rawCount;
                count++;
                _httpContextAccessor.HttpContext.Items[TRANSACTION_COUNT_TOKEN] = count;
            }
            else
            {
                _httpContextAccessor.HttpContext.Items.Add(TRANSACTION_COUNT_TOKEN, 1);
            }
        }

        private void AddTransactionTime(double duration)
        {
            if (_httpContextAccessor.HttpContext == null) return;

            if (_httpContextAccessor.HttpContext.Items.TryGetValue(TRANSACTION_TIME_TOKEN, out object rawTime))
            {
                var time = (double)rawTime;
                time += duration;
                _httpContextAccessor.HttpContext.Items[TRANSACTION_TIME_TOKEN] = time;
            }
            else
            {
                _httpContextAccessor.HttpContext.Items.Add(TRANSACTION_TIME_TOKEN, duration);
            }
        }

        public override DbCommand CommandCreated(CommandEndEventData eventData, DbCommand result)
        {
            AddTransactionTime(eventData.Duration.TotalMilliseconds);

            return base.CommandCreated(eventData, result);
        }

        public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
        {
            AddTransactionTime(eventData.Duration.TotalMilliseconds);
            AddTransactionCount();

            return base.NonQueryExecuted(command, eventData, result);
        }

        public override ValueTask<int> NonQueryExecutedAsync(DbCommand command, CommandExecutedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            AddTransactionTime(eventData.Duration.TotalMilliseconds);
           AddTransactionCount();

            return base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
        }

        public override object ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, object result)
        {
            AddTransactionTime(eventData.Duration.TotalMilliseconds);
            AddTransactionCount();

            return base.ScalarExecuted(command, eventData, result);
        }

        public override ValueTask<object> ScalarExecutedAsync(DbCommand command, CommandExecutedEventData eventData, object result, CancellationToken cancellationToken = default)
        {
            AddTransactionTime(eventData.Duration.TotalMilliseconds);
            AddTransactionCount();

            return base.ScalarExecutedAsync(command, eventData, result, cancellationToken);
        }

        public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
        {
            AddTransactionTime(eventData.Duration.TotalMilliseconds);
            AddTransactionCount();

            return base.ReaderExecuted(command, eventData, result);
        }

        public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
        {
            AddTransactionTime(eventData.Duration.TotalMilliseconds);
            AddTransactionCount();

            return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
        } 
    }
}
