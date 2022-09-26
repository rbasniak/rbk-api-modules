using static rbkApiModules.Utilities.Excel.ExcelModelDefs;

namespace rbkApiModules.Utilities.Excel;

/// <summary>
/// Class describing Hyperlink Data. Needed to write and link all components inside a XML file.
/// </summary>
public class ExcelHyperlink
{
    public ExcelHyperlink()
    {
        LinkId = string.Empty;
    }

    public ExcelHyperlink(string hyperlink)
    {
        Hyperlink = hyperlink;
        LinkId = string.Empty;
    }

    /// <summary>
    /// The real link
    /// </summary>
    public string Hyperlink { get; set; } = string.Empty;

    /// <summary>
    /// Index of the hyperlink that has to be created at worksheet creation time and needed later on the sheet data
    /// </summary>
    public string LinkId { get; set; }
}
