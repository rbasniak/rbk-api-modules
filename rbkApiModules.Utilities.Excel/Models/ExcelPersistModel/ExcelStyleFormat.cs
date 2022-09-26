using System;
using static rbkApiModules.Utilities.Excel.ExcelModelDefs;

namespace rbkApiModules.Utilities.Excel;

/// <summary>
/// Class combining Fonts, Data Type and formats to be applied to Cells or Columns.
/// Each styling is added in dxfs section in the stylesheet xml file combining fills, fonts, data formating, etc.
/// For each diferent combination a TAG is created in the file
/// </summary>
public class ExcelStyleFormat
{
    public ExcelStyleFormat(UInt32 fontIdx, UInt32 numFormatIdx, UInt32 cellStyleIdx, UInt32 fillIdx, UInt32 borderIdx, UInt32 styleIndex)
    {
        FontIndex = fontIdx;
        
        NumFormatIndex = numFormatIdx;
        CellStyleIndex = cellStyleIdx;
        FillIndex = fillIdx;
        BorderIndex = borderIdx;
        StyleIndex = styleIndex;
    }

    /// <summary>
    /// Datatype for the CellXfs style section
    /// </summary>
    public UInt32 FontIndex { get; set; }

    /// <summary>
    /// Possible format string for number or date data types
    /// </summary>
    public UInt32 NumFormatIndex { get; set; }

    /// <summary>
    /// Index of the CellStyle List inside the XML stylestable file. 
    /// Regularly will be 1 for Hyperlink and 0 for everything else. 
    /// </summary>
    public UInt32 CellStyleIndex { get; set; }

    /// <summary>
    /// Index of the Fill Style 
    /// </summary>
    public UInt32 FillIndex { get; set; }

    /// <summary>
    /// Index for the Border Style
    /// </summary>
    public UInt32 BorderIndex { get; set; }

    /// <summary>
    /// This flag is false basically for Hyperlinks and default cells
    /// </summary>
    public bool ApplyFont { get; set; } = true;

    /// <summary>
    /// This flag is true if this style has a NumberFormat added to it
    /// </summary>
    public bool ApplyNumFormat { get; set; } = false;

    /// <summary>
    /// Index of the CellFxs List inside the XML stylestable file
    /// </summary>
    public UInt32 StyleIndex { get; set; }
}
