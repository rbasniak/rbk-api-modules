using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Commons.Localization;

public class RbkAnalyticsModuleOptions
{
    public virtual double TimezoneOffsetHours { get; set; }
    public virtual double IdleInterval { get; set; }
}