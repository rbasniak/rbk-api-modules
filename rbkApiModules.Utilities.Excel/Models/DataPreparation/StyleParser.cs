using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
        string styleKey;
        string key;

        //Run all tables looking for styles
        foreach (var table in workbookModel.Tables)
        {
            key = AddFontToDictionary(_fonts, table.Header.Style, 1);

            styleKey = key + ExcelDataTypes.DataType.Text.ToString();

            AddStyleFormatToDictionary(_styleFormats, styleKey, (UInt32)_fonts[key].FontIndex, 0U, 0U, 0U, 0U, false, true);

            table.Header.AddStyleKey(styleKey);

            foreach (var column in table.Columns)
            {
                key = column.DataType == ExcelDataTypes.DataType.HyperLink ?
                    AddFontToDictionary(_fonts, column.Style, 10) :
                    AddFontToDictionary(_fonts, column.Style, 1);
                // Columns can have diferent types, formats and fonts
                styleKey = key + ExcelDataTypes.DataType.Text.ToString();
                if (column.DataType == ExcelDataTypes.DataType.Text)
                {
                    AddStyleFormatToDictionary(_styleFormats, styleKey, _fonts[key].FontIndex, 0U, 0U, 0U, 0U, false, true);
                    column.AddStyleKey(styleKey);
                }
                else
                {
                    // Add numFormat or CellStyle to xml and get index to add to the style class
                    styleKey = key + column.DataType.ToString();

                    if (column.DataType == ExcelDataTypes.DataType.Number)
                    {
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
                    else if (column.DataType == ExcelDataTypes.DataType.HyperLink)
                    {
                        styleKey = key + column.DataType.ToString();
                        _hyperlinkFormats.TryAdd(key, _fonts[key].FontIndex);
                        AddStyleFormatToDictionary(_styleFormats, styleKey, _fonts[key].FontIndex, 0U, 1U, 0U, 0U, false, false);
                        column.AddStyleKey(styleKey);
                    }
                    else if (column.DataType == ExcelDataTypes.DataType.DateTime)
                    {
                        var sysDateFormat = string.IsNullOrEmpty(column.DataFormat) ? CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern.ToString() : column.DataFormat;
                        var numFormatId = AddNumFormatToDictionary(_numFormats, sysDateFormat);
                        styleKey = styleKey + numFormatId.ToString();
                        AddStyleFormatToDictionary(_styleFormats, styleKey, _fonts[key].FontIndex, numFormatId, 0U, 0U, 0U, true, true);
                        column.AddStyleKey(styleKey);
                    }
                }
            }
        }
        //TODO Add Chart Fonts;

        // Future Watermark Details
        if (workbookModel.Watermark != null)
        {
            var styles = new ExcelStyleClasses()
            {
                Font = workbookModel.Watermark.Font,
                FontSize = workbookModel.Watermark.FontSize
            };

            key = AddFontToDictionary(_fonts, styles, 1);

            styleKey = key + ExcelDataTypes.DataType.Text.ToString();

            AddStyleFormatToDictionary(_styleFormats, key, (UInt32)_fonts[key].FontIndex, 0U, 0U, 0U, 0U, false, true);

            workbookModel.Watermark.AddStyleKey(styleKey);
        }
    }

    private string AddFontToDictionary(
        Dictionary<string, ExcelFontDetail> fonts,
        ExcelStyleClasses styles,
        int colorTheme)
    {
        string key;
        ExcelFontDetail fontDetail;

        key = styles.Font.ToString() + styles.FontSize.ToString() + colorTheme.ToString() + styles.Bold.ToString() + styles.Italic.ToString() + styles.Underline.ToString();
        if (!fonts.ContainsKey(key))
        {
            fontDetail = ExcelFontDetail.GetFontStyles(styles.Font, styles.Bold, styles.Italic, styles.Underline, (UInt32)fonts.Count, styles.FontSize, colorTheme);
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

