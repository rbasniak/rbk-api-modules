using DocumentFormat.OpenXml.Office2010.Word;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using static rbkApiModules.Utilities.Excel.ExcelModelDefs;

namespace rbkApiModules.Utilities.Excel;

internal class StyleParser
{
    private readonly Dictionary<string, UInt32> _styleIndexes;
    private readonly Dictionary<string, ExcelFontDetail> _fonts;
    private readonly Dictionary<string, ExcelNumFormat> _numFormats;
    private readonly Dictionary<string, UInt32> _hyperlinkFormats;
    private readonly Dictionary<string, ExcelStyleFormat> _styleFormats;

    internal StyleParser()
    {
        _styleIndexes = new Dictionary<string, UInt32>();
        _fonts = new Dictionary<string, ExcelFontDetail>();
        _numFormats = new Dictionary<string, ExcelNumFormat>();
        _hyperlinkFormats = new Dictionary<string, uint>();
        _styleFormats = new Dictionary<string, ExcelStyleFormat>();
    }

    internal Dictionary<string, UInt32> StyleIndexes { get { return _styleIndexes; } }
    internal Dictionary<string, ExcelFontDetail> Fonts { get { return _fonts; } }
    internal Dictionary<string, ExcelNumFormat> NumFormats { get { return _numFormats; } }
    internal Dictionary<string, UInt32> HyperlinkFormats { get { return _hyperlinkFormats; } }
    internal Dictionary<string, ExcelStyleFormat> StyleFormats { get { return _styleFormats; } }

    internal void ParseStyles(ExcelWorkbookModel workbookModel)
    {
        //Run all tables looking for styles
        foreach (var table in workbookModel.Tables)
        {
            CreateHeaderStyle(table.Header);

            foreach (var column in table.Columns)
            {
                CreateStylesForeachType(column.DataType, column);
            }
        }
        //TODO Add Chart Fonts;

        if (workbookModel.Watermark != null)
        {
            CreateWatermarkStyle(workbookModel.Watermark);
        }
    }

    private void CreateStylesForeachType(ExcelDataTypes.DataType type, ExcelColumnModel column)
    {
        switch (type)
        {
            case ExcelDataTypes.DataType.HyperLink:
                CreateHyperlinkStyle(column);
                break;
            
            case ExcelDataTypes.DataType.DateTime:
                CreateDatetimeStyle(column);
                break;

            case ExcelDataTypes.DataType.Number:
                CreateNumberStyle(column);
                break;

            case ExcelDataTypes.DataType.Text:
            default:
                CreateTextStyle(column);
                break;
        }
    }

    private void CreateHeaderStyle(ExcelHeaderModel header)
    {
        var key = AddFontToDictionary(_fonts, header.Style, 1);

        var styleKey = key + ExcelDataTypes.DataType.Text.ToString();

        AddStyleFormatToDictionary(_styleFormats, styleKey, (UInt32)_fonts[key].FontIndex, 0U, 0U, 0U, 0U, false, true);

        header.AddStyleKey(styleKey);
    }

    private void CreateHyperlinkStyle(ExcelColumnModel column)
    {
        //Always sets this font color to standard link color
        column.Style.FontColor = string.Empty;
        
        var key = AddFontToDictionary(_fonts, column.Style, 10);

        var styleKey = key + column.DataType.ToString();
        
        _hyperlinkFormats.TryAdd(key, _fonts[key].FontIndex);
        AddStyleFormatToDictionary(_styleFormats, styleKey, _fonts[key].FontIndex, 0U, 1U, 0U, 0U, false, false);
        
        column.AddStyleKey(styleKey);
    }

    private void CreateNumberStyle(ExcelColumnModel column)
    {
        var key = AddFontToDictionary(_fonts, column.Style, 1);

        var styleKey = key + column.DataType.ToString();

        if (!string.IsNullOrEmpty(column.DataFormat))
        {
            var numFormatId = AddNumFormatToDictionary(_numFormats, column.DataFormat);
            styleKey = styleKey + numFormatId.ToString();
            AddStyleFormatToDictionary(_styleFormats, styleKey, _fonts[key].FontIndex, numFormatId, 0U, 0U, 0U, true, true);
        }
        else
        {
            AddStyleFormatToDictionary(_styleFormats, styleKey, _fonts[key].FontIndex, 0U, 0U, 0U, 0U, true, true);
        }
        column.AddStyleKey(styleKey);
    }

    private void CreateDatetimeStyle(ExcelColumnModel column)
    {
        var key = AddFontToDictionary(_fonts, column.Style, 1);

        var styleKey = key + column.DataType.ToString();

        var numFormatId = AddNumFormatToDictionary(_numFormats, column.DataFormat);
        
        styleKey = styleKey + numFormatId.ToString();
        
        AddStyleFormatToDictionary(_styleFormats, styleKey, _fonts[key].FontIndex, numFormatId, 0U, 0U, 0U, true, true);

        column.AddStyleKey(styleKey);
    }

    private void CreateTextStyle(ExcelColumnModel column)
    {
        var key = AddFontToDictionary(_fonts, column.Style, 1);

        var styleKey = key + ExcelDataTypes.DataType.Text.ToString();
        
        AddStyleFormatToDictionary(_styleFormats, styleKey, _fonts[key].FontIndex, 0U, 0U, 0U, 0U, false, true);
        
        column.AddStyleKey(styleKey); 
    }

    private void CreateWatermarkStyle(Watermark watermark)
    {
        var styles = new ExcelStyleClasses()
        {
            Font = watermark.Font,
            FontSize = watermark.FontSize,
            FontColor = watermark.FontColor
        };

        var key = AddFontToDictionary(_fonts, styles, 1);

        var styleKey = key + ExcelDataTypes.DataType.Text.ToString();

        AddStyleFormatToDictionary(_styleFormats, key, (UInt32)_fonts[key].FontIndex, 0U, 0U, 0U, 0U, false, true);

        watermark.AddStyleKey(styleKey);
    }

    private string AddFontToDictionary(
        Dictionary<string, ExcelFontDetail> fonts,
        ExcelStyleClasses styles,
        int colorTheme)
    {
        string key;
        ExcelFontDetail fontDetail;
        
        var regex = new Regex(@"^([A-Fa-f0-9]{8})$");
        
        if (!string.IsNullOrEmpty(styles.FontColor) && regex.IsMatch(styles.FontColor))
        {
            key = styles.Font.ToString() + styles.FontSize.ToString() + styles.FontColor + styles.Bold.ToString() + styles.Italic.ToString() + styles.Underline.ToString();
        }
        else 
        { 
            key = styles.Font.ToString() + styles.FontSize.ToString() + colorTheme.ToString() + styles.Bold.ToString() + styles.Italic.ToString() + styles.Underline.ToString();
        }


        if (!fonts.ContainsKey(key))
        {
            fontDetail = ExcelFontDetail.GetFontStyles(styles.Font, styles.Bold, styles.Italic, styles.Underline, (UInt32)fonts.Count, styles.FontSize, colorTheme, styles.FontColor);
            fonts.Add(key, fontDetail);
        }

        return key;
    }

    private UInt32 AddNumFormatToDictionary(Dictionary<string, ExcelNumFormat> numFormats, string dataFormat)
    {
        UInt32 numFormatId;

        if (numFormats.ContainsKey(dataFormat))
        {
            numFormatId = numFormats[dataFormat].FormatId;
        }
        else
        {
            numFormatId = StyleContants.StartIndex + (UInt32)numFormats.Count;
            ExcelNumFormat numFormat = new ExcelNumFormat(dataFormat, numFormatId);
            numFormats.Add(dataFormat, numFormat);
        }

        return numFormatId;
    }

    private void AddStyleFormatToDictionary(
        Dictionary<string, ExcelStyleFormat> styleFormats,
        string styleKey,
        UInt32 fontIdx,
        UInt32 numFormatIdx,
        UInt32 cellStyleIdx,
        UInt32 fillIdx,
        UInt32 borderIdx,
        bool applyNumFormat,
        bool applyFont)
    {
        ExcelStyleFormat styleFormat;

        if (!styleFormats.ContainsKey(styleKey))
        {
            styleFormat = new ExcelStyleFormat(fontIdx, numFormatIdx, cellStyleIdx, fillIdx, borderIdx, (UInt32)styleFormats.Count() + 1);
            styleFormat.ApplyFont = applyFont;
            styleFormat.ApplyNumFormat = applyNumFormat;
            styleFormats.Add(styleKey, styleFormat);
            _styleIndexes.Add(styleKey, styleFormat.StyleIndex);
        }
    }

    
}

