using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Commons.Localization;

public interface IAnalyticModuleStore
{
    void StoreData(AnalyticsEntry request);
    void StoreSession(SessionEntry session);

    Task<List<AnalyticsEntry>> FilterStatisticsAsync(DateTime from, DateTime to, string[] versions, string[] areas, string[] domains,
        string[] actions, string[] users, string[] agents, int[] responses, string[] methods, int duration, string entityId);
    Task<List<AnalyticsEntry>> FilterStatisticsAsync(DateTime from, DateTime to);
    Task<List<AnalyticsEntry>> GetStatisticsAsync();
    Task<FilterOptionListData> GetFilteringLists();
    Task<List<SessionEntry>> GetSessionsAsync(DateTime dateFrom, DateTime dateTo);
    Task<List<PerformanceEntry>> FilterPerformanceData(string endpoint, DateTime dateFrom, DateTime dateTo);
    void DeleteStatisticsFromMatchingPathAsync(string searchText);
    void NormalizePathsAndActions();
}