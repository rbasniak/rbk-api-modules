using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using x14 = DocumentFormat.OpenXml.Office2010.Excel;
using x15 = DocumentFormat.OpenXml.Office2013.Excel;
using DocumentFormat.OpenXml.ExtendedProperties;
using System.Text.RegularExpressions;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using rbkApiModules.Infrastructure.Models;

namespace rbkApiModules.Utilities.Excel;

public class SaxLib
{
    public MemoryStream CreatePackage(ExcelWorkbookModel workbookModel)
    {
        DataParser parser = new DataParser();
        StyleParser styleParser = new StyleParser(); ;

        var stream = new MemoryStream();

        using (SpreadsheetDocument document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
        {
            document.AddWorkbookPart();

            if (document.WorkbookPart == null)
            {
                throw new SafeException("Error creating workbook part");
            }

            // Clean all strings of non encoded caracters
            parser.SanitizeData(workbookModel);

            // Generate all Shared Strings that will be used in all the sheets
            parser.PrepareData(workbookModel);

            // Generate all Styles needed on every sheet in this workbook
            var stylesPartId = "sPrId1";
            var sharedTableId = "sTrId1";
            
            WorkbookStylesPart workbookStylesPart = document.WorkbookPart.AddNewPart<WorkbookStylesPart>(stylesPartId);
            SharedStringTablePart sharedStringTablePart = document.WorkbookPart.AddNewPart<SharedStringTablePart>(sharedTableId);
            GenerateStylePart(workbookStylesPart, workbookModel, parser, styleParser);
            GenerateSharedStringsTable(sharedStringTablePart, parser);

            var partId = 1;
            var linksId = 1;
            List<string> sheetPartIds = new List<string>();
            var numSheets = workbookModel.Tables.Count();

            for (int sheetNum = 1; sheetNum <= numSheets; sheetNum++)
            {
                var sheetPartId = $"rId{partId++}";
                sheetPartIds.Add(sheetPartId);

                WorksheetPart workSheetPart = document.WorkbookPart.AddNewPart<WorksheetPart>(sheetPartId);
                TableDefinitionPart sheetTablesPart = workSheetPart.AddNewPart<TableDefinitionPart>(sheetPartId);

                var sheetModel = workbookModel.Tables[sheetNum - 1];
                var allColumns = sheetModel.Columns;
                var linkColumns = allColumns.Where(x => x.DataType == ExcelModelDefs.ExcelDataTypes.DataType.HyperLink).ToList();
                try
                {
                    foreach (var linkColumn in linkColumns)
                    {
                        linksId = GenerateHyperlinkParts(workSheetPart, linkColumn, linksId);
                    }
                }
                catch
                {
                    throw new SafeException("The hyperlinks on table " + sheetModel.Name + " must start with http:// or https://.");
                }

                int numRows = allColumns.Select(x => x.Data.Length).Max() + sheetModel.StartRow;

                GenerateWorkSheetData(workSheetPart, sheetModel, allColumns, numRows, sheetPartId, parser, styleParser);
                GenerateTableParts(sheetTablesPart, (UInt32)sheetNum, sheetModel.Header, sheetModel.Theme, sheetModel.StartRow, numRows);
            }

            // Create the worksheet and sheets list to end the package
            LinkAllSheetsInformations(document.WorkbookPart, workbookModel, numSheets, sheetPartIds);

            SetDocumentProperties(workbookModel, document);

            document.Save();

            document.Close();
        }
        return stream;
    }

    private void SetDocumentProperties(ExcelWorkbookModel workbookModel, SpreadsheetDocument document)
    {
        var corePart = document.AddCoreFilePropertiesPart();

        using (System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(corePart.GetStream(System.IO.FileMode.Create), System.Text.Encoding.UTF8))
        {
            writer.WriteRaw(
                "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>\r\n" +
                "<cp:coreProperties xmlns:cp=\"http://schemas.openxmlformats.org/package/2006/metadata/core-properties\" " +
                "xmlns:dc=\"http://purl.org/dc/elements/1.1/\" " +
                "xmlns:dcterms=\"http://purl.org/dc/terms/\" " +
                "xmlns:dcmitype=\"http://purl.org/dc/dcmitype/\" " +
                "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" +
                $"<dc:description>{workbookModel.Comments}</dc:description>" +
                $"<dc:title>{workbookModel.Title}</dc:title>" +
                $"<dc:creator>{workbookModel.Author}</dc:creator>" +
                $"<dcterms:created xsi:type=\"dcterms:W3CDTF\">{DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")}</dcterms:created>" +
                "</cp:coreProperties>");
            writer.Flush();
            writer.Close();
        }
        
        if (!string.IsNullOrEmpty(workbookModel.Company))
        {
            document.AddExtendedFilePropertiesPart();
            if (document.ExtendedFilePropertiesPart != null)
            {
                document.ExtendedFilePropertiesPart.Properties = new Properties();
                document.ExtendedFilePropertiesPart.Properties.Company = new Company(workbookModel.Company);
            }
        }
    }

    private void LinkAllSheetsInformations(WorkbookPart workbookPart, ExcelWorkbookModel workbookModel, int numSheets, List<string> sheetPartIds)
    {
        using (var writer = OpenXmlWriter.Create(workbookPart))
        {
            writer.WriteStartElement(new Workbook());

            writer.WriteStartElement(new BookViews());
            writer.WriteElement(new WorkbookView());
            writer.WriteEndElement();

            writer.WriteStartElement(new Sheets());

            for (int sheetNum = 1; sheetNum <= numSheets; sheetNum++)
            {
                writer.WriteElement(new Sheet()
                {
                    Name = workbookModel.Tables[sheetNum - 1].Name,
                    SheetId = (UInt32)sheetNum,
                    Id = sheetPartIds[sheetNum - 1]
                });
            }

            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.Close();
        }
    }

    private int GenerateHyperlinkParts(WorksheetPart workSheetPart, ExcelColumnModel linkColumn, int partIdSequencer)
    {
        foreach (var link in linkColumn.HyperLinkData)
        {
            if (!string.IsNullOrEmpty(link.Hyperlink))
            {
                var id = $"lId{partIdSequencer++}";
                workSheetPart.AddHyperlinkRelationship(new Uri(link.Hyperlink, UriKind.Absolute), true, id);
                link.LinkId = id;
            }
        }
        return partIdSequencer;
    }

    private void GenerateSharedStringsTable(SharedStringTablePart sharedStringTablePart, DataParser parser)
    {
        using (var writer = OpenXmlWriter.Create(sharedStringTablePart))
        {
            var stringCount = parser.GetSharedStringCount();
            writer.WriteStartElement(new SharedStringTable() { Count = stringCount.TotalCount, UniqueCount = stringCount.UniqueCount });

            foreach (var key in parser.GetSharedStrings())
            {
                writer.WriteStartElement(new SharedStringItem());
                writer.WriteElement(new Text(key));
                writer.WriteEndElement();
            }

            writer.WriteEndElement();

            writer.Close();
        }
    }

    private void GenerateWorkSheetData(WorksheetPart workSheetPart, ExcelTableSheetModel sheetModel, ExcelColumnModel[] allColumns, int numRows, string sheetPartId, DataParser parser, StyleParser styleParser)
    {
        using (var writer = OpenXmlWriter.Create(workSheetPart))
        {
            var headers = sheetModel.Header;
            var numColumns = allColumns.Count();
                        
            writer.WriteStartElement(new Worksheet());

            if (!string.IsNullOrEmpty(sheetModel.TabColor))
            {
               WriteSheetTabColorSection(writer, sheetModel);
            }

            FreezeHeaderPane(writer, sheetModel.StartRow);

            WriteSheetColumnDescriptionSection(writer, numColumns, headers, allColumns);

            writer.WriteStartElement(new SheetData());

            if(sheetModel.StartRow > 1)
            {
                WriteSheetDataSubtotalRow(writer, sheetModel.StartRow, numColumns, numRows, allColumns, styleParser);
            }
            
            WriteSheetDataHeaderRow(writer, sheetModel.StartRow, numColumns, headers, parser, styleParser);

            WriteSheetDataColumns(writer, sheetModel.StartRow, numColumns, numRows, allColumns, parser, styleParser);

            // write the end SheetData element
            writer.WriteEndElement();

            //HyperlinksInfo
            WriteHyperlinkSection(writer, sheetModel.StartRow, numColumns, allColumns);

            // Table Info
            writer.WriteStartElement(new TableParts() { Count = 1 });
            writer.WriteElement(new TablePart() { Id = sheetPartId });
            writer.WriteEndElement();

            // write the end Worksheet element
            writer.WriteEndElement();
            
            writer.Close();
        }
    }

    private void WriteSheetTabColorSection(OpenXmlWriter writer, ExcelTableSheetModel sheetModel)
    {
        var regex = new Regex(ExcelModelDefs.Configuration.ColorPattern);
        if (!regex.IsMatch(sheetModel.TabColor))
        {
            throw new SafeException("Invalid Color String. String should be a 8 characters ARGB string without the #. e.g. \"FF00FF00\" for solid green");
        }
    
        writer.WriteStartElement(new SheetProperties());

        writer.WriteElement(new TabColor() { Rgb = HexBinaryValue.FromString(sheetModel.TabColor) });

        writer.WriteEndElement();
    }

    private void FreezeHeaderPane(OpenXmlWriter writer, int startRow)
    {
        writer.WriteStartElement(new SheetViews());
        writer.WriteStartElement(new SheetView()
        {
            WorkbookViewId = 0
        });

        writer.WriteElement(new Pane() 
        {
            VerticalSplit = startRow, 
            TopLeftCell = $"A{startRow+1}", 
            ActivePane = PaneValues.BottomLeft, 
            State = PaneStateValues.Frozen 
        });
        writer.WriteElement(new Selection() 
        { 
            Pane = PaneValues.BottomLeft, 
            ActiveCell = $"A{startRow + 1}", 
            SequenceOfReferences = new ListValue<StringValue>() 
            { 
                InnerText = $"A{startRow + 1}" 
            } 
        });

        writer.WriteEndElement();
        writer.WriteEndElement();
    }

    private void WriteSheetColumnDescriptionSection(OpenXmlWriter writer, int numColumns, ExcelHeaderModel headers, ExcelColumnModel[] allColumns)
    {
        writer.WriteStartElement(new Columns());

        for (int columnNum = 1; columnNum <= numColumns; columnNum++)
        {
            var column = allColumns[columnNum - 1];
            
            var width = FitColumn(headers.Data[columnNum - 1], headers.Style, column, column.IsMultilined, column.MaxWidth);
            
            writer.WriteElement(new Column() { Min = (UInt32)columnNum, Max = (UInt32)columnNum, Width = width, CustomWidth = true });
        }
        
        writer.WriteEndElement();
    }

    private void WriteSheetDataSubtotalRow(OpenXmlWriter writer, int startRow, int numColumns, int numRows, ExcelColumnModel[] allColumns, StyleParser styleParser)
    {
        Cell cell = new Cell();
        CellFormula cellFormula = new CellFormula();
        
        writer.WriteStartElement(new Row() { RowIndex = 1U });

        for (int columnNum = 1; columnNum <= numColumns; columnNum++)
        {
            if (allColumns[columnNum - 1].HasSubtotal)
            {
                cell.CellReference = $"{GetColumnName(columnNum)}{startRow-1}";
                cell.DataType = CellValues.SharedString;
                cell.StyleIndex = styleParser.StyleIndexes[allColumns[columnNum - 1].StyleKey];
                writer.WriteStartElement(cell);

                var columnName = GetColumnName(columnNum);
                cellFormula.Text = $"SUBTOTAL(9, {columnName}{startRow+1}:{columnName}${numRows})";
                writer.WriteElement(cellFormula);

                writer.WriteEndElement();
            }
        }

        writer.WriteEndElement();
    }

    private void WriteSheetDataHeaderRow(OpenXmlWriter writer, int startRow, int numColumns, ExcelHeaderModel headers, DataParser parser, StyleParser styleParser)
    {
        Cell cell = new Cell();
        CellValue cellValue = new CellValue();
        if (headers.RowHeight > 12)
        {
            writer.WriteStartElement(new Row() { RowIndex = (UInt32)startRow, Height = headers.RowHeight, CustomHeight = true });
        }
        else
        {
            writer.WriteStartElement(new Row() { RowIndex = (UInt32)startRow });
        }

        for (int columnNum = 1; columnNum <= numColumns; columnNum++)
        {
            cell.CellReference = $"{ GetColumnName(columnNum)}{(UInt32)startRow}";

            cell.DataType = CellValues.SharedString;
            cell.StyleIndex = styleParser.StyleIndexes[headers.StyleKey];
            writer.WriteStartElement(cell);
            cellValue.Text = parser.GetValue(ExcelModelDefs.ExcelDataTypes.DataType.Text, headers.Data[columnNum - 1]);
            writer.WriteElement(cellValue);

            writer.WriteEndElement();
        }

        writer.WriteEndElement();
    }

    private void WriteSheetDataColumns(OpenXmlWriter writer, int startRow, int numColumns, int numRows, ExcelColumnModel[] allColumns, DataParser parser, StyleParser styleParser)
    {
        Row row = new Row();
        Cell cell = new Cell();
        CellValue cellValue = new CellValue();

        var rowShift = startRow + 1;

        for (int rowNum = rowShift; rowNum <= numRows; rowNum++)
        {
            //write the row start element with the row index attribute
            row.RowIndex = (UInt32)rowNum;
            writer.WriteStartElement(row);

            for (int columnNum = 1; columnNum <= numColumns; columnNum++)
            {
                var columnIndex = columnNum - 1;
                var rowIndex = rowNum - rowShift;
                var currentColumn = allColumns[columnIndex];
                if (allColumns.Length > (columnIndex) && allColumns[columnIndex].Data.Length > (rowIndex))
                {
                    cell.CellReference = $"{GetColumnName(columnNum)}{rowNum}";
                    cell.StyleIndex = styleParser.StyleIndexes[currentColumn.StyleKey];

                    SetCellValue(currentColumn, cell, cellValue, allColumns, rowIndex, columnIndex, parser, styleParser);

                    writer.WriteStartElement(cell);
                    writer.WriteElement(cellValue);
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
        }
    }

    private void SetCellValue(ExcelColumnModel currentColumn, Cell cell, CellValue cellValue, ExcelColumnModel[] allColumns, int rowIndex, int columnIndex, DataParser parser, StyleParser styleParser)
    {
        cellValue.Text = parser.GetValue(currentColumn.DataType, allColumns[columnIndex].Data[rowIndex]);
        switch (currentColumn.DataType)
        {
            case ExcelModelDefs.ExcelDataTypes.DataType.Number:
                cell.DataType = CellValues.Number;
                break;
            
            case ExcelModelDefs.ExcelDataTypes.DataType.DateTime:
                cell.DataType = CellValues.Number;
                break;

            case ExcelModelDefs.ExcelDataTypes.DataType.HyperLink:
            case ExcelModelDefs.ExcelDataTypes.DataType.Text:
            default:
                cell.DataType = CellValues.SharedString;
                break;
        }
    }

    private void WriteHyperlinkSection(OpenXmlWriter writer, int startRow, int numColumns, ExcelColumnModel[] allColumns)
    {
        var rowShift = startRow + 1;
        if (allColumns.Any(x => x.DataType == ExcelModelDefs.ExcelDataTypes.DataType.HyperLink))
        {
            writer.WriteStartElement(new Hyperlinks());
            
            for (int columnNum = 1; columnNum <= numColumns; columnNum++)
            {
                if (allColumns[columnNum - 1].DataType == ExcelModelDefs.ExcelDataTypes.DataType.HyperLink)
                {
                    var linkColumn = allColumns[columnNum - 1];
                    var hyperlink = new Hyperlink();
            
                    for (int rowNum = 2; rowNum <= linkColumn.HyperLinkData.Length + 1; rowNum++)
                    {
                        if (!string.IsNullOrEmpty(linkColumn.HyperLinkData[rowNum - rowShift].Hyperlink))
                        {
                            hyperlink.Reference =$"{GetColumnName(columnNum)}{rowNum}";
                            hyperlink.Id = linkColumn.HyperLinkData[rowNum - rowShift].LinkId;
                            writer.WriteElement(hyperlink);
                        }
                    }

                }
            }
            writer.WriteEndElement();
        }
    }

    private void GenerateTableParts(TableDefinitionPart sheetTablesPart, UInt32 tableId, ExcelHeaderModel headers, ExcelModelDefs.ExcelThemes.Theme theme, int startRow, int numRows)
    {
        var numColumns = headers.Data.Count();

        using (var writer = OpenXmlWriter.Create(sheetTablesPart))
        {
            var reference = $"A{startRow}:{GetColumnName(numColumns)}{numRows}";
            var tableName = $"Table{tableId}";
            var table = new Table()
            {
                Id = tableId,
                Name = tableName,
                DisplayName = tableName,
                Reference = reference,
                TotalsRowShown = false,
            };
            // Start Table element
            writer.WriteStartElement(table);

            writer.WriteElement(new AutoFilter() { Reference = reference });

            writer.WriteStartElement(new TableColumns() { Count = (UInt32)numColumns });

            for (int columnNum = 1; columnNum <= numColumns; columnNum++)
            {
                writer.WriteElement(new TableColumn()
                {
                    Id = (UInt32)columnNum,
                    Name = headers.Data[columnNum - 1]
                });
            }

            writer.WriteEndElement();

            writer.WriteElement(new TableStyleInfo()
            {
                Name = ExcelModelDefs.ExcelThemes.GetTheme(theme),
                ShowFirstColumn = false,
                ShowLastColumn = false,
                ShowRowStripes = true,
                ShowColumnStripes = false
            });

            //End Table
            writer.WriteEndElement();
            writer.Close();
        }
    }

    // Everything is linked by a string id that is in fact the index of the array of style element. Ex the font with id "2"
    // will be the third font added in fonts section, while the font with id "0" will be the first you added.
    // Same goes for borders, fills, etc.
    private void GenerateStylePart(WorkbookStylesPart workbookStylesPart, ExcelWorkbookModel workbookModel, DataParser parser, StyleParser styleParser)
    {
        styleParser.ParseStyles(workbookModel);

        using (var writer = OpenXmlWriter.Create(workbookStylesPart))
        {
            writer.WriteStartElement(new Stylesheet());

            #region NumFormats

            writer.WriteStartElement(new NumberingFormats() { Count = (UInt32)styleParser.NumFormats.Count });
            
            foreach (var format in styleParser.NumFormats.Values)
            {
                writer.WriteElement(new NumberingFormat() { NumberFormatId = format.FormatId, FormatCode = format.FormatCode });  
            }

            writer.WriteEndElement();

            #endregion

            #region Fonts

                //write the fonts sections
                //<Fonts>
                //  <Font>...props...</Font>
                //</Fonts>
                //writer.WriteStartElement(new Fonts() { Count = (UInt32)hardCodedFonts.Length });
                writer.WriteStartElement(new Fonts() { Count = (UInt32)styleParser.Fonts.Count });

            foreach (var font in styleParser.Fonts.Values)
            {
                writer.WriteStartElement(new Font());
                if (font.Bold)
                {
                    writer.WriteElement(new Bold());
                }
                if (font.Italic)
                {
                    writer.WriteElement(new Italic());
                }
                if (font.Underline)
                {
                    writer.WriteElement(new Underline());
                }
                writer.WriteElement(new FontSize() { Val = font.FontSize });
                if (string.IsNullOrEmpty(font.FontColor))
                {
                    writer.WriteElement(new Color() { Theme = font.Theme });
                }
                else
                {
                    writer.WriteElement(new Color() { Rgb = HexBinaryValue.FromString(font.FontColor) });
                }
                writer.WriteElement(new FontName() { Val = font.FontName });
                writer.WriteElement(new FontFamily() { Val = font.FontFamily });

                //Close the single Font Tag
                writer.WriteEndElement();
            }

            // End Fonts section
            writer.WriteEndElement();

            #endregion

            #region Fills

            //Hardcoded Props
            var fills = new PatternValues[2] { PatternValues.None, PatternValues.Gray125 };

            writer.WriteStartElement(new Fills() { Count = (UInt32)fills.Length });

            foreach (var fill in fills)
            {
                writer.WriteStartElement(new Fill());

                writer.WriteElement(new PatternFill() { PatternType = fill });

                //Close the single Font Tag
                writer.WriteEndElement();
            }

            // End Fills section
            writer.WriteEndElement();

            #endregion

            #region Borders

            //Hardcoded Props
            var borderCount = 1;
            // Start Borders section
            writer.WriteStartElement(new Borders() { Count = (UInt32)borderCount });
            //Start border element
            writer.WriteStartElement(new Border());

            writer.WriteElement(new LeftBorder());
            writer.WriteElement(new RightBorder());
            writer.WriteElement(new TopBorder());
            writer.WriteElement(new BottomBorder());
            writer.WriteElement(new DiagonalBorder());

            //Close the boder
            writer.WriteEndElement();
            // End Borders section
            writer.WriteEndElement();

            #endregion

            #region CellStyleXfs (Cell Style Formats)

            // Creates a shared style table to apply to cells using an Id. 

            var cellStyleXfsCount = styleParser.HyperlinkFormats.Count + 1;
            //Start CellStyleXfs element
            writer.WriteStartElement(new CellStyleFormats() { Count = (UInt32)cellStyleXfsCount });
            //Hardcoded base CellFormat
            writer.WriteElement(new CellFormat() { NumberFormatId = (UInt32)0, FontId = (UInt32)0, FillId = (UInt32)0, BorderId = (UInt32)0 });

            foreach(var fontIndex in styleParser.HyperlinkFormats.Values)
            {
                writer.WriteElement(new CellFormat()
                {
                    NumberFormatId = (UInt32)0,
                    FontId = fontIndex,
                    FillId = (UInt32)0,
                    BorderId = (UInt32)0,
                    ApplyNumberFormat = false,
                    ApplyFill = false,
                    ApplyBorder = false,
                    ApplyAlignment = false,
                    ApplyProtection = false
                });
            }
            
            // End CellStyleXfs section
            writer.WriteEndElement();

            #endregion

            #region CellXfs (CellFormats)
            
            // Add all alignment and apply numberformat features
            
            var cellXfsCount = styleParser.StyleFormats.Count() + 1;

            //Start CellStyleFormats section
            writer.WriteStartElement(new CellFormats() { Count = (UInt32)cellXfsCount });

            writer.WriteElement(new CellFormat() { NumberFormatId = (UInt32)0, FontId = (UInt32)0, FillId = (UInt32)0, BorderId = (UInt32)0 });

            foreach (var styleFormat in styleParser.StyleFormats.Values)
            {
                writer.WriteStartElement(new CellFormat() 
                { 
                    NumberFormatId = styleFormat.NumFormatIndex,
                    FontId = styleFormat.FontIndex, 
                    FillId = (UInt32)0, 
                    BorderId = (UInt32)0, 
                    ApplyFont = styleFormat.ApplyFont,
                    ApplyNumberFormat = styleFormat.ApplyNumFormat,
                    ApplyAlignment = true,
                    
                });
                writer.WriteElement(new Alignment() 
                {
                    Horizontal = HorizontalAlignmentValues.Left,
                    Vertical = VerticalAlignmentValues.Center, 
                    WrapText = true 
                });
                //End CellXf
                writer.WriteEndElement();
            }
            // End CellStyleFormats section
            writer.WriteEndElement();

            #endregion

            #region CellStyles

            var cellStylesCount = styleParser.HyperlinkFormats.Count() + 1;

            //Start CellStyleFormats element
            writer.WriteStartElement(new CellStyles() { Count = (UInt32)cellStylesCount });

            writer.WriteElement(new CellStyle() { Name = "Normal", FormatId = (UInt32)0, BuiltinId = (UInt32)0 });
            if (styleParser.HyperlinkFormats.Count() > 0)
            {
                writer.WriteElement(new CellStyle() { Name = "Hyperlink", FormatId = (UInt32)1, BuiltinId = (UInt32)8 });
            }
            // End CellStyles section
            writer.WriteEndElement();

            #endregion

            #region Diferential formats

            //Hardcoded Props
            var diferentialFormatsCount = 0;
            // Start diferential formats section Although empty it is a needed part of the Stylesheet
            writer.WriteStartElement(new DifferentialFormats() { Count = (UInt32)diferentialFormatsCount });
            writer.WriteEndElement();

            #endregion

            #region TableStyles

            writer.WriteElement(new TableStyles() { Count = 0, DefaultTableStyle = "TableStyleMedium2", DefaultPivotStyle = "PivotStyleLight16" });

            #endregion

            #region Style Extensions List

            //Start extensions list
            writer.WriteStartElement(new StylesheetExtensionList());

            var guid = "{" + Guid.NewGuid() + "}";
            writer.WriteStartElement(new StylesheetExtension() { Uri = guid });
            writer.WriteElement(new x14.SlicerStyles() { DefaultSlicerStyle = "SlicerStyleLight1" });
            writer.WriteEndElement();

            guid = "{" + Guid.NewGuid() + "}";
            writer.WriteStartElement(new StylesheetExtension() { Uri = guid });
            writer.WriteElement(new x15.TimelineStyles() { DefaultTimelineStyle = "TimeSlicerStyleLight1" });
            writer.WriteEndElement();

            // End extensions list
            writer.WriteEndElement();

            #endregion

            //End styleSsheet
            writer.WriteEndElement();
            writer.Close();
        }
    }

    private double FitColumn(string header, ExcelStyleClasses headerStyle, ExcelColumnModel column, bool isMultilined, int maxWidth)
    {
        var hOffset = 2;
        var cOffset = 0;
        

        var hFontFactor = ExcelModelDefs.ExcelFonts.GetFontSizeFactor(headerStyle.Font);
        var cFontFactor = ExcelModelDefs.ExcelFonts.GetFontSizeFactor(column.Style.Font);
        var fontRatio = (double)column.Style.FontSize / (double)headerStyle.FontSize;

        if (column.DataType == ExcelModelDefs.ExcelDataTypes.DataType.DateTime)
        {
            cOffset = 5;
        }

        if (headerStyle.Bold)
        {
            hFontFactor -= 1;
        }

        if (column.Style.Bold == true)
        {
            cFontFactor -= 0.5D;
        }

        double headerWidth = (header.Length + hOffset) * (72D / 96D) * (headerStyle.FontSize / hFontFactor);
        double columnWidth = (column.GetMaxDataLength(isMultilined, ExcelModelDefs.Configuration.NumLengthSamples) + cOffset) * (72D / 96D) * column.Style.FontSize/ cFontFactor;

        var width = headerWidth >= columnWidth ? headerWidth : columnWidth;

        if (maxWidth > 13)
        {
            var higherFontSize = headerStyle.FontSize > column.Style.FontSize ? headerStyle.FontSize : column.Style.FontSize;
            var correctedMaxWidth = maxWidth * (72D / 96D) * (higherFontSize / hFontFactor) * fontRatio;
            width = width > correctedMaxWidth ? correctedMaxWidth : width;
        }

        return width;
    }

    //A simple helper to get the column name from the column index. This is not well tested!
    private string GetColumnName(int columnIndex)
    {
        int dividend = columnIndex;
        string columnName = String.Empty;
        int modifier;

        while (dividend > 0)
        {
            modifier = (dividend - 1) % 26;
            columnName = Convert.ToChar(65 + modifier).ToString() + columnName;
            dividend = (int)((dividend - modifier) / 26);
        }

        return columnName;
    }
}
