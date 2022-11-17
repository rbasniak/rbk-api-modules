
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
    /// Sets the text color. Expects an 8 characters Hexadecimal ARGB string pattern without the # e.g.
    /// "FFFF0000" for solid Red."FF00FF00" for solid green and "FF0000FF" for solid blue. 
    /// Default is "66000000" for solid Black.
    /// </summary>
    public string FontColor { get; set; } = "66000000";

    /// <summary>
    /// The Font that will be used to write watermark
    /// </summary>
    public ExcelModelDefs.ExcelFonts.FontType Font { get; set; } = ExcelModelDefs.ExcelFonts.FontType.Calibri;

    /// <summary>
    /// Font size for writing the watermark
    /// </summary>
    public int FontSize { get; set; } = 40;

    /// <summary>
    /// The angle for which the watermark should be rotated
    /// </summary>
    public int RotationAngle { get; set; } = 0;

    

    #region helper fields and methods

    /// <summary>
    /// Reserved Quick access Key Built from font, fontSize, data type and format
    /// </summary>
    public string StyleKey { get; private set; } = "";

    public void AddStyleKey(string key)
    {
        StyleKey = key;
    }

    #endregion
}
