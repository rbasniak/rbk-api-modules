using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace rbkApiModules.Utilities.Excel;

/// <summary>
/// Helper class that parses data into dictionaries that can be stored on excel files as indexes.
/// </summary>
internal class ExcelSharedString
{
    private readonly Dictionary<string, string> _sharedStringsToIndex;
    private int _sharedStringsTotalCount;
    
    internal ExcelSharedString()
    {
        _sharedStringsToIndex = new Dictionary<string, string>();
    }
    
    internal Dictionary <string, string> SharedStringsToIndex { get { return _sharedStringsToIndex; } }

    internal string GetValue(string key)
    {
        return _sharedStringsToIndex[key];
    }

    internal SharedStringCount GetSharedStringCount()
    {
        return new SharedStringCount((UInt32)_sharedStringsTotalCount, (UInt32)_sharedStringsToIndex.Count());
    }

    internal void AddToSharedStringDictionary(string[] sharedStrings, bool isMultilined, string newLine)
    {
        var count = 0;
        for (int itemIndex = 0; itemIndex < sharedStrings.Length; itemIndex++)
        {
            if (isMultilined)
            {
                sharedStrings[itemIndex] = Regex.Replace(sharedStrings[itemIndex], newLine, Environment.NewLine, RegexOptions.IgnoreCase);
            }

            if (_sharedStringsToIndex.ContainsKey(sharedStrings[itemIndex]))
            {
                count++;
            }
            else
            {
                count++;
                _sharedStringsToIndex.Add(sharedStrings[itemIndex], _sharedStringsToIndex.Count().ToString());
            }
        }
        _sharedStringsTotalCount += count;
    }
}
