using System;

namespace rbkApiModules.Utilities.Excel;

/// <summary>
/// Class describing a Font as it is written inside the XML file.
/// </summary>
public class ExcelNumFormat
{ 
    public ExcelNumFormat(string formatCode, UInt32 formatId)
    {
        FormatCode = formatCode;
        FormatId = formatId;
    }

    /// <summary>
    /// FontName as in Excel font list
    /// </summary>
    public string FormatCode { get; set; } = string.Empty;

    /// <summary>
    /// Index of the Font inside the XML stylestable file
    /// </summary>
    public UInt32 FormatId { get; set; }
}
