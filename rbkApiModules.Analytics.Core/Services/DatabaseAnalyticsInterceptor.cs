using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace rbkApiModules.Analytics.Core
{
    public class DatabaseAnalyticsInterceptor : DbCommandInterceptor
    { 
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DatabaseAnalyticsInterceptor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private void AddTransactionCount()
        {
            if (_httpContextAccessor.HttpContext == null) return;

            if (_httpContextAccessor.HttpContext.Items.TryGetValue("transaction-count", out object rawCount))
            {
                var count = (int)rawCount;
                count++;
                _httpContextAccessor.HttpContext.Items["transaction-count"] = count;
            }
            else
            {
                _httpContextAccessor.HttpContext.Items.Add("transaction-count", 1);
            }
        }

        private void AddTransactionTime(int duration)
        {
            if (_httpContextAccessor.HttpContext == null) return;

            if (_httpContextAccessor.HttpContext.Items.TryGetValue("transaction-time", out object rawTime))
            {
                var time = (int)rawTime;
                time  += duration;
                _httpContextAccessor.HttpContext.Items["transaction-time"] = time;
            }
            else
            {
                _httpContextAccessor.HttpContext.Items.Add("transaction-time", duration);
            }
        }

        public override DbCommand CommandCreated(CommandEndEventData eventData, DbCommand result)
        {
            AddTransactionTime((int)eventData.Duration.TotalMilliseconds);

            return base.CommandCreated(eventData, result);
        }

        public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
        {
            AddTransactionTime((int)eventData.Duration.TotalMilliseconds);
            AddTransactionCount();

            return base.NonQueryExecuted(command, eventData, result);
        }

        public override ValueTask<int> NonQueryExecutedAsync(DbCommand command, CommandExecutedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            AddTransactionTime((int)eventData.Duration.TotalMilliseconds);
            AddTransactionCount();

            return base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
        }

        public override object ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, object result)
        {
            AddTransactionTime((int)eventData.Duration.TotalMilliseconds);
            AddTransactionCount();

            return base.ScalarExecuted(command, eventData, result);
        }

        public override ValueTask<object> ScalarExecutedAsync(DbCommand command, CommandExecutedEventData eventData, object result, CancellationToken cancellationToken = default)
        {
            AddTransactionTime((int)eventData.Duration.TotalMilliseconds);
            AddTransactionCount();

            return base.ScalarExecutedAsync(command, eventData, result, cancellationToken);
        }

        public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
        {
            AddTransactionTime((int)eventData.Duration.TotalMilliseconds);
            AddTransactionCount();

            return base.ReaderExecuted(command, eventData, result);
        }

        public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
        {
            AddTransactionTime((int)eventData.Duration.TotalMilliseconds);
            AddTransactionCount();

            return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
        } 
    }
}
