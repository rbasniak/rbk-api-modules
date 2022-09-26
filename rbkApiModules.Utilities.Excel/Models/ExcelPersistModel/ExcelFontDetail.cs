using System;
using static rbkApiModules.Utilities.Excel.ExcelModelDefs;

namespace rbkApiModules.Utilities.Excel;

/// <summary>
/// Class describing a Font as it is written inside the XML file.
/// </summary>
public class ExcelFontDetail
{ 
    public ExcelFontDetail(string fontName, UInt32 fontSize, bool bold, bool italic, bool underline,  Int32 fontFamily, UInt32 theme, UInt32 fontIndex)
    {
        FontName = fontName;
        FontSize = fontSize;
        FontFamily = fontFamily;
        Theme = theme;
        FontIndex = fontIndex;
        Bold = bold;
        Italic = italic;
        Underline = underline;
    }

    public ExcelFontDetail(string fontName, UInt32 fontSize, bool bold, bool italic, bool underline, Int32 fontFamily, string fontColor, UInt32 fontIndex)
    {
        FontName = fontName;
        FontSize = fontSize;
        FontFamily = fontFamily;
        FontColor = fontColor;
        FontIndex = fontIndex;
        Bold = bold;
        Italic = italic;
        Underline = underline;
    }

    /// <summary>
    /// FontName as in Excel font list
    /// </summary>
    public string FontName { get; set; } = "Calibri";

    /// <summary>
    /// Font Size in Points as in Excel font size dropdown
    /// </summary>
    public UInt32 FontSize { get; set; } = 11;

    /// <summary>
    /// Set the font as bold
    /// </summary>
    public bool Bold { get; set; }

    /// <summary>
    /// Set the font as Italic
    /// </summary>
    public bool Italic { get; set; }

    /// <summary>
    /// Set an underline when using this font
    /// </summary>
    public bool Underline { get; set; }

    /// <summary>
    /// Font family number as defined by Excel Microsoft Arial, Calibri, Times are family 2
    /// </summary>
    public Int32 FontFamily { get; set; } = 2;

    /// <summary>
    /// Theme for custom microsoft themes (Not table style theme) This is a diferent theme
    /// </summary>
    public UInt32 Theme { get; set; } = 1;

    /// <summary>
    /// If a hex value is set, then the font will preferably use the Hex color value. If none is set, it will use the theme color
    /// Expects an 8 characters Hexadecimal ARGB string pattern without the # e.g. "FFFF0000" for solid Red.
    /// "FF00FF00" for solid green and "FF0000FF" for solid blue.
    /// </summary>
    public string FontColor { get; set; } = string.Empty;

    /// <summary>
    /// Index of the Font inside the XML stylestable file
    /// </summary>
    public UInt32 FontIndex { get; set; }

    public static ExcelFontDetail GetFontStyles(ExcelFonts.FontType font, bool bold, bool italic, bool underline, UInt32 fontIndex, int fontSize, int theme, string fontColor = "")
    {
        if (string.IsNullOrEmpty(fontColor))
        {
            switch ((int)font)
            {
                case 0: return new ExcelFontDetail("Arial", (UInt32)fontSize, bold, italic, underline, 2, (UInt32)theme, fontIndex);
                case 1: return new ExcelFontDetail("Calibri", (UInt32)fontSize, bold, italic, underline, 2, (UInt32)theme, fontIndex);
                case 2: return new ExcelFontDetail("Calibri Light", (UInt32)fontSize, bold, italic, underline, 2, (UInt32)theme, fontIndex);
                case 3: return new ExcelFontDetail("Courrier New", (UInt32)fontSize, bold, italic, underline, 1, (UInt32)theme, fontIndex);
                case 4: return new ExcelFontDetail("Times New Roman", (UInt32)fontSize, bold, italic, underline, 2, (UInt32)theme, fontIndex);
                default:
                    return new ExcelFontDetail("Calibri", (UInt32)fontSize, bold, italic, underline, 2, (UInt32)theme, fontIndex);
            }
        }
        else
        {
            switch ((int)font)
            {
                case 0: return new ExcelFontDetail("Arial", (UInt32)fontSize, bold, italic, underline, 2, fontColor, fontIndex);
                case 1: return new ExcelFontDetail("Calibri", (UInt32)fontSize, bold, italic, underline, 2, fontColor, fontIndex);
                case 2: return new ExcelFontDetail("Calibri Light", (UInt32)fontSize, bold, italic, underline, 2, fontColor, fontIndex);
                case 3: return new ExcelFontDetail("Courrier New", (UInt32)fontSize, bold, italic, underline, 1, fontColor, fontIndex);
                case 4: return new ExcelFontDetail("Times New Roman", (UInt32)fontSize, bold, italic, underline, 2, fontColor, fontIndex);
                default:
                    return new ExcelFontDetail("Calibri", (UInt32)fontSize, bold, italic, underline, 2, fontColor, fontIndex);
            }
        }
    }

}
