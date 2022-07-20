using System;
using System.Drawing;

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
    /// String representing the color name. Ex: "Black", "Red", "Gray", etc.
    /// </summary>
    public string TextColor { get; set; } = "Black";
    /// <summary>
    /// The Font that will be used to write watermark
    /// </summary>
    public ClosedXMLDefs.ExcelFonts.FontName FontName { get; set; } = ClosedXMLDefs.ExcelFonts.FontName.Calibri;
    /// <summary>
    /// The angle for which the watermark should be rotated
    /// </summary>
    public int RotationAngle { get; set; } = 0;
    /// <summary>
    /// Font size for writing the watermark
    /// </summary>
    public int FontSize { get; set; } = 40;
}
