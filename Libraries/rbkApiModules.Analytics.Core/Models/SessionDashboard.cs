using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Commons.Localization;

public class SessionsDashboard
{
    public object TotalDailyUseTime { get; set; }
    public object TotalAccumulatedUseTime { get; set; }

    public object DailyUseTimePerUser { get; set; }
    public object AccumulatedUseTimePerUser { get; set; }

    public object AverageSessionTime { get; set; }

    public object DailySessions { get; set; }

    // Tempo total de uso por usuario(diário / acumulado) por nome de usuario 
}