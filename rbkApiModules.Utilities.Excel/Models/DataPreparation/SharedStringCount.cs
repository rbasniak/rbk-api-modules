using System;

namespace rbkApiModules.Utilities.Excel;

internal class SharedStringCount
{
    internal SharedStringCount(UInt32 totalCount, UInt32 uniqueCount)
    {
        TotalCount = totalCount;
        UniqueCount = uniqueCount;
    }

    internal UInt32 UniqueCount { get; set; }
    internal UInt32 TotalCount { get; set; }
}
