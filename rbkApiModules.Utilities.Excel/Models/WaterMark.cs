using static rbkApiModules.Utilities.Excel.ExcelModelDefs;

namespace rbkApiModules.Utilities.Excel;

/// <summary>
/// Class holding all required data for generating a watermark image
/// </summary>
public class Watermark
{
    /// <summary>
    /// The Text that will be displayed as watermark image
    /// </summary>
    public string Text { get; set; } = string.Empty;
    /// <summary>
    /// Transparency level from 0 (solid) to 255 (invisible)
    /// </summary>
    public float Alpha { get; set; } = 0;
    /// <summary>
    /// Sets the text color using an Enum based on System.Drawing Color names. Ex: "Black", "Red", "Gray", etc.
    /// </summary>
    public ExcelColors.Color TextColor { get; set; } = ExcelColors.Color.Black;
    /// <summary>
    /// The Font that will be used to write watermark
    /// </summary>
    public ExcelFonts.FontType Font { get; set; } = ExcelFonts.FontType.Calibri;
    /// <summary>
    /// The angle for which the watermark should be rotated
    /// </summary>
    public int RotationAngle { get; set; } = 0;
    /// <summary>
    /// Font size for writing the watermark
    /// </summary>
    public int FontSize { get; set; } = 40;
    /// <summary>
    /// Reserved Quick access Key Built from font, fontSize, data type and format
    /// </summary>
    public string StyleKey { get; private set; } = "";

    public void AddStyleKey(string key)
    {
        StyleKey = key;
    }
}
