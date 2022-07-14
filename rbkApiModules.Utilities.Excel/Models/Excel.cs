using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rbkApiModules.Utilities.Excel;

public class Excel
{
    protected Excel() {}

    public Excel(string json)
    {
        ExcelJson = json;
    }

    public virtual string ExcelJson { get; set; }
}
