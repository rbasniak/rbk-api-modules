using static rbkApiModules.Utilities.Excel.ClosedXMLDefs;

namespace rbkApiModules.Utilities.Excel;

/// <summary>
/// Class representing a style sheet for the excel headers and column data
/// </summary>
public class ExcelStyleClasses
{
    /// <summary>
    /// Font that should be applied on the data set. Ex:"0" for Arial; "3" for Calibri, "8" for Georgia Pro, etc
    /// </summary>
    public ExcelFonts.FontName Font { get; set; } = ExcelFonts.FontName.Calibri;
    /// <summary>
    /// Font size following the excel app standard integer size
    /// </summary>
    public int FontSize { get; set; } = 11;
    /// <summary>
    /// If bold property should be applied
    /// </summary>
    public bool Bold { get; set; } = false;
    /// <summary>
    /// If Italic property should be applied
    /// </summary>
    public bool Italic { get; set; } = false;
    /// <summary>
    /// The data sets Data Type. Ex: "0" for Number, "1" for Text, "2" for DateTime, etc.
    /// </summary>
    public ExcelDataTypes.DataType DataType { get; set; } = ExcelDataTypes.DataType.Text;
    /// <summary>
    /// Data formating for a specific type. Ex: For Number Type: DataFormat = "0.00" Will format the number with 2 decimal precision.
    /// For the DateTime type: DataFormat = "dd/MM/yyyy" will format dates for the brazilian standard.
    /// </summary>
    public string DataFormat { get; set; } = string.Empty;
    /// <summary>
    /// Excell Max Column Width. If this property is set to a positive value, it will limit the Column to the MaxWidth.
    /// Useful for large texts that often ocupy large portions the monitor's width.
    /// </summary>
    public int MaxWidth { get; set; } = -1;
}
