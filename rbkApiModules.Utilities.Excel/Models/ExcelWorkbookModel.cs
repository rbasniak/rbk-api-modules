using static rbkApiModules.Utilities.Excel.ClosedXMLDefs;

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
    /// If diferent from "None", applies a theme from excel's standard theme list
    /// </summary>
    public ExcelThemes.Theme Theme { get; set; } = ExcelThemes.Theme.None;
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
    /// Authoring Metadata, creation date
    /// </summary>
    public string DateCreated { get; set; } = string.Empty;
    /// <summary>
    /// Authoring Metadata, Comments
    /// </summary>
    public string Comments { get; set; } = string.Empty;
    /// <summary>
    /// If the workbook is a Draft or a Release
    /// </summary>
    public bool IsDraft { get; set; } = false;
    /// <summary>
    /// The data to generate a watermark image
    /// </summary>
    public Watermark? Watermark { get; set; }
    /// <summary>
    /// List of all spreadsheets for this workbook, with their data and styling.
    /// </summary>
    public ExcelSheetModel[]? Sheets { get; set; }
}
