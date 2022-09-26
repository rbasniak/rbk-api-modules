using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace rbkApiModules.Utilities.Excel;

/// <summary>
/// Helper class that parses data into dictionaries that can be stored on excel files as indexes.
/// </summary>
internal class ExcelDate
{
    private readonly Dictionary<string, string> _oleADates;
        
    internal ExcelDate()
    {
        _oleADates = new Dictionary<string, string>();
    }

    internal string DateFormat { get; set; } = string.Empty;

    internal Dictionary<string, string> OleADates { get { return _oleADates; } }

    internal bool IsDate(ExcelColumnModel column, string format)
    {
        if (string.IsNullOrEmpty(format.Trim()))
        {
            return false;
        }    
        if (column.Data.Any(x => !string.IsNullOrEmpty(x) && x.Length != format.Length))
        {
            return false;
        }
        
        return true;
    }
    
    internal string GetValue(string key)
    {
        if (_oleADates.TryGetValue(key, out var oleADate))
        {
            return oleADate;
        }

        return string.Empty;
    }

    internal void AddToDatetimeToDictionary(string[] dates, string dataFormat)
    {
        var index = 0;
        DateTime date;
        while (index < dates.Length)
        {
            if (!string.IsNullOrEmpty(dates[index].Trim()) && !_oleADates.ContainsKey(dates[index]))
            {
                if (DateTime.TryParseExact(dates[index], dataFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                {
                    _oleADates.Add(dates[index], date.ToOADate().ToString());
                }
                else
                {
                    throw new Exception("Unsupported date format");
                }
            }
            index++;
        }
    }

    
}
