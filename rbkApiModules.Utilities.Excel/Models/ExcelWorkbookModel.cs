using System.Collections.Generic;
using System.Linq;

namespace rbkApiModules.Utilities.Excel;

/// <summary>
/// The Main Workbook Model container. Holds all data and metadata.
/// </summary>
public class ExcelWorkbookModel
{
    /// <summary>
    /// Name of the excel file containing all the spreadsheets to be outputed
    /// </summary>
    public string FileName { get; set; } = "ExcelFile.xlsx";
    /// <summary>
    /// Authoring Metadata, Title
    /// </summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>
    /// Authoring Metadata, Author name
    /// </summary>
    public string Author { get; set; } = string.Empty;
    
    /// <summary>
    /// Authoring Metadata, Company name
    /// </summary>
    public string Company { get; set; } = string.Empty;
    
    /// <summary>
    /// Authoring Metadata, Comments
    /// </summary>
    public string Comments { get; set; } = string.Empty;

    /// <summary>
    /// This class must contain all rules needed for finding specific data types when autodetect is true for a column.
    /// </summary>
    public ExcelGlobalBehavior GlobalColumnBehavior { get; set; } = new ExcelGlobalBehavior();

    /// <summary>
    /// The data to generate a watermark image
    /// </summary>
    public Watermark Watermark { get; set; }
    
    /// <summary>
    /// List of all spreadsheets for this workbook, with tabular data and styling.
    /// </summary>
    public ExcelTableSheetModel[] Tables { get; set; } = new ExcelTableSheetModel[0]; 
    
    /// <summary>
    /// List of all plot sheets for this workbook, with plot, their data and styling.
    /// </summary>
    public ExcelChartSheetModel[] Charts { get; set; } = new ExcelChartSheetModel[0];
    
    public IEnumerable<ExcelBaseSheetModel> AllSheets
    {
        get
        {
            var allSheets = new List<ExcelBaseSheetModel>();
            if (Tables != null)
            {
                allSheets.AddRange(Tables);
            }
            if (Charts != null)
            {
                allSheets.AddRange(Charts);
            }
            return allSheets.OrderBy(x => x.TabIndex);
        }
    }
}
