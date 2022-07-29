using static rbkApiModules.Utilities.Excel.ClosedXMLDefs;

namespace rbkApiModules.Utilities.Excel;

/// <summary>
/// Class with the data models and styling for a column data, to be displayed under a header title.
/// </summary>
public class ExcelColumnModel
{
    /// <summary>
    /// List of all data to be displayed on one column
    /// </summary>
    public string[] Data { get; set; }
    /// <summary>
    /// Styles to be applied to this column's data
    /// </summary>
    public ExcelStyleClasses Style { get; set; }
    /// <summary>
    /// The data sets Data Type. Ex: "0" for Text, "1" for Number, "2" for DateTime, etc.
    /// </summary>
    public ExcelDataTypes.DataType DataType { get; set; } = ExcelDataTypes.DataType.Text;
    /// <summary>
    /// Data formating for a specific type. Ex: For Number Type: DataFormat = "0.00" Will format the number with 2 decimal precision.
    /// For the DateTime type: DataFormat = "dd/MM/yyyy" will format dates for the brazilian standard.
    /// </summary>
    public string DataFormat { get; set; } = string.Empty;
    /// <summary>
    /// Excel Max Column Width in POINT units (Not Pixels). If this property is set to a positive value, it will limit the Column to the MaxWidth.
    /// Useful for large texts that often ocupy large portions the monitor's width.
    /// </summary>
    public int MaxWidth { get; set; } = -1;
}
