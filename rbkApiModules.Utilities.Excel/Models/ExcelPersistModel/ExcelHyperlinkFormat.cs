using System;

namespace rbkApiModules.Utilities.Excel;

/// <summary>
/// Class describing a Font as it is written inside the XML file.
/// </summary>
public class ExcelHyperlinkFormat
{ 
    public ExcelHyperlinkFormat(UInt32 fontIdx)
    {
        FontIndex = fontIdx;
    }

    /// <summary>
    /// FontName as in Excel font list
    /// </summary>
    public UInt32 CellStyleId { get; set; }

    /// <summary>
    /// Index of the Font inside the XML stylestable file
    /// </summary>
    public UInt32 FontIndex { get; set; }
}
