using DocumentFormat.OpenXml.Office2010.Word;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

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
            table.Header = CreateHeaderStyle(table.Header);

            for (int columnNumber = 0; columnNumber < table.Columns.Length; columnNumber++)
            {
                var column = table.Columns[columnNumber];
                table.Columns[columnNumber] = CreateStylesForeachType(column.DataType, column);
            }
        }
        //TODO Add Chart Fonts;

        if (workbookModel.Watermark != null)
        {
            workbookModel.Watermark = CreateWatermarkStyle(workbookModel.Watermark);
        }
    }

    private ExcelColumnModel CreateStylesForeachType(ExcelModelDefs.ExcelDataTypes.DataType type, ExcelColumnModel column)
    {
        switch (type)
        {
            case ExcelModelDefs.ExcelDataTypes.DataType.HyperLink:
                return CreateHyperlinkStyle(column);
                
            case ExcelModelDefs.ExcelDataTypes.DataType.DateTime:
                return CreateDatetimeStyle(column);
            
            case ExcelModelDefs.ExcelDataTypes.DataType.Number:
                return CreateNumberStyle(column);

            case ExcelModelDefs.ExcelDataTypes.DataType.Text:
            default:
                return CreateTextStyle(column);
        }
    }

    private ExcelHeaderModel CreateHeaderStyle(ExcelHeaderModel header)
    {
        var styledHeader = header;

        var key = AddFontToDictionary(header.Style, 1);

        var styleKey = key + ExcelModelDefs.ExcelDataTypes.DataType.Text.ToString();

        AddStyleFormatToDictionary(styleKey, (UInt32)_fonts[key].FontIndex, 0U, 0U, 0U, 0U, false, true);

        styledHeader.AddStyleKey(styleKey);

        return header;
    }

    private ExcelColumnModel CreateHyperlinkStyle(ExcelColumnModel column)
    {
        var styledColumn = column;

        //Always sets this font color to standard link color
        column.Style.FontColor = string.Empty;
        
        var key = AddFontToDictionary(column.Style, 10);

        var styleKey = key + column.DataType.ToString();
        
        _hyperlinkFormats.TryAdd(key, _fonts[key].FontIndex);
        AddStyleFormatToDictionary(styleKey, _fonts[key].FontIndex, 0U, 1U, 0U, 0U, false, false);

        styledColumn.AddStyleKey(styleKey);

        return styledColumn;
    }

    private ExcelColumnModel CreateNumberStyle(ExcelColumnModel column)
    {
        var styledColumn = column;

        var key = AddFontToDictionary(column.Style, 1);

        var styleKey = key + column.DataType.ToString();

        if (!string.IsNullOrEmpty(column.DataFormat))
        {
            var numFormatId = AddNumFormatToDictionary(column.DataFormat);
            styleKey = styleKey + numFormatId.ToString();
            AddStyleFormatToDictionary(styleKey, _fonts[key].FontIndex, numFormatId, 0U, 0U, 0U, true, true);
        }
        else
        {
            AddStyleFormatToDictionary(styleKey, _fonts[key].FontIndex, 0U, 0U, 0U, 0U, true, true);
        }
        styledColumn.AddStyleKey(styleKey);

        return styledColumn;
    }

    private ExcelColumnModel CreateDatetimeStyle(ExcelColumnModel column)
    {
        var styledColumn = column;

        var key = AddFontToDictionary(column.Style, 1);

        var styleKey = key + column.DataType.ToString();

        var numFormatId = AddNumFormatToDictionary(column.DataFormat);
        
        styleKey = styleKey + numFormatId.ToString();
        
        AddStyleFormatToDictionary(styleKey, _fonts[key].FontIndex, numFormatId, 0U, 0U, 0U, true, true);

        styledColumn.AddStyleKey(styleKey);

        return styledColumn;
    }

    private ExcelColumnModel CreateTextStyle(ExcelColumnModel column)
    {
        var styledColumn = column;

        var key = AddFontToDictionary(column.Style, 1);

        var styleKey = key + ExcelModelDefs.ExcelDataTypes.DataType.Text.ToString();
        
        AddStyleFormatToDictionary(styleKey, _fonts[key].FontIndex, 0U, 0U, 0U, 0U, false, true);

        styledColumn.AddStyleKey(styleKey);

        return styledColumn;
    }

    private Watermark CreateWatermarkStyle(Watermark watermark)
    {
        var styledWatermark = watermark;

        var styles = new ExcelStyleClasses()
        {
            Font = watermark.Font,
            FontSize = watermark.FontSize,
            FontColor = watermark.FontColor
        };

        var key = AddFontToDictionary(styles, 1);

        var styleKey = key + ExcelModelDefs.ExcelDataTypes.DataType.Text.ToString();

        AddStyleFormatToDictionary(key, (UInt32)_fonts[key].FontIndex, 0U, 0U, 0U, 0U, false, true);

        styledWatermark.AddStyleKey(styleKey);

        return styledWatermark;
    }

    private string AddFontToDictionary(ExcelStyleClasses styles, int colorTheme)
    {
        string key;
        ExcelFontDetail fontDetail;
        
        var regex = new Regex(ExcelModelDefs.Configuration.ColorPattern);
        
        if (!string.IsNullOrEmpty(styles.FontColor) && regex.IsMatch(styles.FontColor))
        {
            key = styles.Font.ToString() + styles.FontSize.ToString() + styles.FontColor + styles.Bold.ToString() + styles.Italic.ToString() + styles.Underline.ToString();
        }
        else 
        { 
            key = styles.Font.ToString() + styles.FontSize.ToString() + colorTheme.ToString() + styles.Bold.ToString() + styles.Italic.ToString() + styles.Underline.ToString();
        }


        if (!_fonts.ContainsKey(key))
        {
            fontDetail = ExcelFontDetail.GetFontStyles(styles.Font, styles.Bold, styles.Italic, styles.Underline, (UInt32)_fonts.Count, styles.FontSize, colorTheme, styles.FontColor);
            _fonts.Add(key, fontDetail);
        }

        return key;
    }

    private UInt32 AddNumFormatToDictionary(string dataFormat)
    {
        UInt32 numFormatId;

        if (_numFormats.ContainsKey(dataFormat))
        {
            numFormatId = _numFormats[dataFormat].FormatId;
        }
        else
        {
            numFormatId = ExcelModelDefs.StyleContants.StartIndex + (UInt32)_numFormats.Count;
            ExcelNumFormat numFormat = new ExcelNumFormat(dataFormat, numFormatId);
            _numFormats.Add(dataFormat, numFormat);
        }

        return numFormatId;
    }

    private void AddStyleFormatToDictionary(
        string styleKey,
        UInt32 fontIdx,
        UInt32 numFormatIdx,
        UInt32 cellStyleIdx,
        UInt32 fillIdx,
        UInt32 borderIdx,
        bool applyNumFormat,
        bool applyFont)
    {
        if (!_styleFormats.ContainsKey(styleKey))
        {
            var styleFormat = new ExcelStyleFormat(fontIdx, numFormatIdx, cellStyleIdx, fillIdx, borderIdx, (UInt32)_styleFormats.Count() + 1);
            styleFormat.ApplyFont = applyFont;
            styleFormat.ApplyNumFormat = applyNumFormat;
            _styleFormats.Add(styleKey, styleFormat);
            _styleIndexes.Add(styleKey, styleFormat.StyleIndex);
        }
    }

    
}

