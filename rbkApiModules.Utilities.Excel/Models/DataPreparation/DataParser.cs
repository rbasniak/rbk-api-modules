using rbkApiModules.Infrastructure.Models;
using System.Linq;
using static rbkApiModules.Utilities.Excel.ExcelModelDefs;

namespace rbkApiModules.Utilities.Excel;

/// <summary>
/// Helper class that parses data into dictionaries that can be stored on excel files as indexes.
/// </summary>
internal class DataParser
{
    private ExcelDate _excelDate;
    private ExcelHyperlinkParser _hyperlinkParser;
    private ExcelSharedString _sharedString;
    
    internal DataParser()
    {
        _excelDate = new ExcelDate();
        _hyperlinkParser = new ExcelHyperlinkParser();
        _sharedString = new ExcelSharedString();
    }

    internal SharedStringCount GetSharedStringCount()
    {
        return _sharedString.GetSharedStringCount();
    }

    internal string[] GetSharedStrings()
    {
        return _sharedString.SharedStringsToIndex.Keys.ToArray();
    }

    internal string GetValue(ExcelDataTypes.DataType type, string key)
    {
        switch (type)
        {
            case ExcelDataTypes.DataType.Text:
            case ExcelDataTypes.DataType.HyperLink:
                return _sharedString.GetValue(key);

            case ExcelDataTypes.DataType.DateTime:
                return _excelDate.GetValue(key);

            default:
                return key;
        }
    }

    internal void PrepareData(ExcelWorkbookModel workbookModel)
    {
        foreach (var table in workbookModel.Tables)
        {
            if (table.Header.Data.Length != table.Columns.Length)
            {
                throw new SafeException("Length of Headers and number of columns must match");
            }

            _sharedString.AddToSharedStringDictionary(table.Header.Data, false, string.Empty);
            
            foreach (var column in table.Columns)
            {
                SetupColumn(table, column, workbookModel.GlobalColumnBehavior);

                switch (column.DataType)
                {
                    case ExcelDataTypes.DataType.DateTime:
                        _excelDate.AddToDatetimeToDictionary(column.Data, column.DataFormat);
                        break;
                    case ExcelDataTypes.DataType.HyperLink:
                        _hyperlinkParser.PrepareHyperlinks(column, workbookModel.GlobalColumnBehavior.Hyperlink.IsHtml, column.IsMultilined);
                        _sharedString.AddToSharedStringDictionary(column.Data, column.IsMultilined, column.NewLineString);
                        break;
                    case ExcelDataTypes.DataType.Number:
                    case ExcelDataTypes.DataType.Text:
                    default:
                        _sharedString.AddToSharedStringDictionary(column.Data, column.IsMultilined, column.NewLineString);
                        break;
                }
            }
        }
    }

    private void SetupColumn(ExcelTableSheetModel table, ExcelColumnModel column, ExcelGlobalBehavior globalBehavior)
    {
        if (column.HasSubtotal)
        {
            table.SetStartRow(2);
        }

        if (!string.IsNullOrEmpty(column.NewLineString.Trim()))
        {
            column.IsMultilined = true;
        }

        if (column.DataType == ExcelDataTypes.DataType.DateTime && string.IsNullOrEmpty(column.DataFormat.Trim()))
        {
            if (!string.IsNullOrEmpty(globalBehavior.Date.Format.Trim()))
            {
                column.DataFormat = globalBehavior.Date.Format;
            }
            else
            {
                throw new SafeException("No Date Format found");
            }
        }

        if (column.DataType == ExcelDataTypes.DataType.AutoDetect)
        {
            DetermineDataType(column, globalBehavior, column.IsMultilined);
        }
    }

    private ExcelDataTypes.DataType DetermineDataType(ExcelColumnModel column, ExcelGlobalBehavior behavior, bool isMultilined)
    {
        if (_hyperlinkParser.IsHyperlink(column, behavior.Hyperlink.IsHtml))
        {
            column.DataType = ExcelDataTypes.DataType.HyperLink;
            return column.DataType;
        }

        // Multilined columns should not detect dates
        if (!isMultilined)
        {
            if (_excelDate.IsDate(column, behavior.Date.Format))
            {
                column.DataType = ExcelDataTypes.DataType.DateTime;
                column.DataFormat = behavior.Date.Format;
                return column.DataType;
            }
        }

        column.DataType = ExcelDataTypes.DataType.Text;
        return column.DataType;
    }
}
