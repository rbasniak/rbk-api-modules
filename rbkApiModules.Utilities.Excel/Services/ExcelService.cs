﻿
using ClosedXML.Excel;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using static rbkApiModules.Utilities.Excel.ClosedXMLDefs;

namespace rbkApiModules.Utilities.Excel;

public interface IExcelService
{
    ExcelsDetails GenerateSpreadsheetTablesFromWorkbookModel(ExcelWorkbookModel workbookModel);
    FileDto GenerateSpreadsheetTablesFromWorkbookModelAsFile(ExcelWorkbookModel workbookModel);
    public MemoryStream GenerateSpreadsheetTablesFromWorkbookModelAsStream(ExcelWorkbookModel workbookModel);
}

public class ExcelService : IExcelService
{
    public FileDto GenerateSpreadsheetTablesFromWorkbookModelAsFile(ExcelWorkbookModel workbookModel)
    {
        var stream = GenerateSpreadsheetTablesFromWorkbookModelAsStream(workbookModel);
        // Create the FileDto to be returned as stream to the client
        var file = new FileDto()
        {
            FileName = workbookModel.FileName + ".xlsx",
            ContentType = "application/vnd.ms-excel",
            FileStream = stream
        };
    
        return file;
    }
    public ExcelsDetails GenerateSpreadsheetTablesFromWorkbookModel(ExcelWorkbookModel workbookModel)
    {
        var stream = GenerateSpreadsheetTablesFromWorkbookModelAsStream(workbookModel);

        // Create the FileDto to be returned as stream to the client
        var file = new ExcelsDetails()
        {
            FileName = workbookModel.FileName,
            FileExtension = "xlsx",
            File = Convert.ToBase64String(stream.ToArray())
        };
        
        return file;
    }

    public MemoryStream GenerateSpreadsheetTablesFromWorkbookModelAsStream(ExcelWorkbookModel workbookModel)
    {
        var stream = new MemoryStream();

        using (var workbook = new XLWorkbook())
        {
            //Setup Workbook Metadata properties
            SetWorkbookMetadata(workbookModel, workbook);

            //Begin adding the sheets with it's data
            foreach (var model in workbookModel.AllSheets)
            {
            // Add the worksheet 
                var worksheet = AddWorksheet(workbook, model.Name);

                if (model.SheetType == ExcelSheetTypes.Type.Table)
                {
                    AddTableDataToSheet(model as ExcelTableSheetModel, worksheet);
                }
                else if (model.SheetType == ExcelSheetTypes.Type.Chart)
                {
                    // TODO: Add plot handling methods
                    continue;
                }        
            }
            

            if (workbookModel.IsDraft == true)
            {
                var watermarkModel = workbookModel.Watermark;
                if (watermarkModel != null)
                {
                    // call drawtext() to create an image
                    var imageWatermark = DrawText(
                        watermarkModel.Text,
                        watermarkModel.Font,
                        watermarkModel.FontSize,
                        watermarkModel.TextColor,
                        watermarkModel.Alpha,
                        watermarkModel.RotationAngle,
                        800,
                        800);
                    // Add the watermark to each worksheet
                    foreach (var worksheet in workbook.Worksheets)
                    {
                        var rangeUsed = worksheet.RangeUsed();
                        CreateWatermarkInWorksheet(imageWatermark, rangeUsed, worksheet);

                    }
                }
            }

            //Save the excel workbook to a memory stream
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
    }

    private IXLWorksheet AddWorksheet(IXLWorkbook workbook, string worksheetName)
    {
        var worksheet = workbook.Worksheets.Add(worksheetName);
        worksheet.PageSetup.PageOrientation = XLPageOrientation.Landscape;
        worksheet.PageSetup.PaperSize = XLPaperSize.A4Paper;
        // worksheet.PageSetup.FitToPages(1, 1);

        return worksheet;
    }

    private void AddTableDataToSheet(ExcelTableSheetModel model, IXLWorksheet worksheet)
    {
        //Validate basic data before adding any content to the worksheet
        if (model != null && model.Header != null && model.Header.Data != null && model.Columns != null)
        {
            // Determine table size (Max column based on header data count;
            var maxColumnCount = model.Header.Data.Length;

            // Helper range variable for quick access to the headers
            var headerRange = worksheet.Range(1, 1, 1, maxColumnCount);
            foreach (var (cell, i) in headerRange.Cells().Select((value, i) => (value, i)))
            {
                cell.Value = model.Header.Data[i];
            }

            //Apply header styles
            SetConfigurationAndStyling(model.Header.Style, headerRange);
            worksheet.Row(1).AdjustToContents();
            // Place the column data for each column
            foreach (var (column, i) in model.Columns.Select((value, i) => (value, i)))
            {
                // i starts at zero and columns at one so it needs to be incremented
                // Rows have to jump the headers rows so it is also incremented by one
                if (column.Data != null)
                {
                    var columnRange = worksheet.Range(2, i + 1, column.Data.Length + 1, i + 1);
                    foreach (var (cell, j) in columnRange.Cells().Select((value, j) => (value, j)))
                    {
                        AddValue(cell, column.Data[j], column.DataType, column.DataFormat);
                    }
                    // Apply data typing and styling
                    var ixlColumn = worksheet.Column(i + 1);
                    SetConfigurationAndStyling(column.Style, columnRange);
                    SetCellSizeAndWrapText(column, ixlColumn);
                }
            }

            // Freeze the header row
            worksheet.SheetView.FreezeRows(1);

            // fetch the range with the used cells
            var rangeUsed = worksheet.RangeUsed();

            // fetch the inner range for data only
            var columnCount = rangeUsed.ColumnCount();
            var rowCount = rangeUsed.RowCount();
            if (columnCount > 1)
            {
                var innerRange = worksheet.Range(2, 1, rowCount, columnCount);

                if (model.ShouldSort)
                {
                    if (rangeUsed.Columns().Count() >= model.SortColumn)
                    {
                        innerRange.Sort(model.SortColumn, ExcelSort.GetSortOrder(model.SortOrder), model.MatchCase, model.IgnoreBlanks);
                    }
                }

                // Setup a theme.
                SetThemeForSpreadsheet(model, worksheet, columnCount, rowCount);
            }
        }
    }

    private void SetThemeForSpreadsheet(ExcelTableSheetModel model, IXLWorksheet worksheet, int columnCount, int rowCount)
    {
        // Important!!!! Themes come with autofiltering enabled.
        // For that reason, auto-filtering should only be enabled manualy if no theme was applied;
        var rangeUsed = worksheet.RangeUsed();
        if (model.Theme != ExcelThemes.Theme.None)
        {
            var table = rangeUsed.CreateTable();
            table.Theme = ExcelThemes.GetTheme(model.Theme);
            //Set tab colors
            if (model.TabColor != ExcelColors.Color.NoColor)
            { 
                worksheet.SetTabColor(ExcelColors.GetColor(model.TabColor));
            }
            SetVerticalDataSeparator(worksheet, columnCount, rowCount);
            worksheet.ShowGridLines = false;
        }
        else
        {
            rangeUsed.SetAutoFilter(true);
            worksheet.ShowGridLines = true;
        }
    }

private void AddValue(IXLCell cell, string value, ExcelDataTypes.DataType dataType, string dataFormat)
    {
        // Try to parse and add data as date type
        if (dataType == ExcelDataTypes.DataType.DateTime)
        {
            bool parsed;
            DateTime date;
            if (dataFormat != null)
            {
                parsed = DateTime.TryParseExact(value, dataFormat, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out date);

            }
            else
            {
                parsed = DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out date);
            }
            if (parsed == true)
            {
                cell.Value = date;
                cell.DataType = ExcelDataTypes.GetDataType(dataType);
                if (dataFormat != null)
                {
                    cell.Style.DateFormat.Format = dataFormat;
                }
            }
            else
            {
                cell.Value = value;
            }
        }
        // Create Hyperlinks
        else if (dataType == ExcelDataTypes.DataType.HyperLink)
        {
            cell.Value = value; 
            cell.SetHyperlink(new XLHyperlink(cell));
        }
        // Create any other data type and try to add possible format string
        else
        {
            cell.DataType = ExcelDataTypes.GetDataType(dataType);
            if (dataType == ExcelDataTypes.DataType.Number)
            {
                if (dataFormat != null)
                {
                    cell.Style.NumberFormat.Format = dataFormat;
                }
            }
            cell.Value = value;
        }

    }

    private void SetVerticalDataSeparator(IXLWorksheet worksheet, int columnCount, int rowCount)
    {
        var innerRange = worksheet.Range(2, 1, rowCount, columnCount - 1);
        innerRange.Style.Border.RightBorder = XLBorderStyleValues.Hair;
        //innerRange.Style.Border.RightBorderColor = (XLColor.FromTheme(XLThemeColor.Accent3));
    }

    private void SetWorkbookMetadata(ExcelWorkbookModel workbookModel, XLWorkbook workbook)
    {
        workbook.Properties.Title = workbookModel.Info;
        workbook.Properties.Author = workbookModel.Author;
        
        workbook.Properties.Created = DateTime.UtcNow;
                
        workbook.Properties.Company = workbookModel.Company;
        workbook.Properties.Comments = workbookModel.Comments;
    }

    private void SetConfigurationAndStyling(ExcelStyleClasses styles, IXLRange ixlRange)
    {
        // Set standard alignment
        ixlRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        ixlRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

        // Setup fonts
        ixlRange.Style.Font.FontName = ExcelFonts.GetFontName(styles.Font);
        ixlRange.Style.Font.FontSize = styles.FontSize;
        ixlRange.Style.Font.Bold = styles.Bold;
        ixlRange.Style.Font.Italic = styles.Italic;
    }

    private void SetCellSizeAndWrapText(ExcelColumnModel model, IXLColumn ixlColumn)
    {
        // Adjust width and height to content
        ixlColumn.AdjustToContents();
        if (model.MaxWidth != -1 && ixlColumn.Width > model.MaxWidth)
        {
            ixlColumn.Width = model.MaxWidth;
        }
        else
        {
            //Give some room for the autofilter dropdowns that will overlap the text
            ixlColumn.Width += 4;
        }
        // If column data type is text then extend wraptext to the whole column
        if (model.DataType == ExcelDataTypes.DataType.Text)
        {
            ixlColumn.Style.Alignment.WrapText = true;
        }
    }

    private void CreateWatermarkInWorksheet(Stream imageWatermark, IXLRange rangeUsed, IXLWorksheet worksheet)
    {
        double totalColumnsWidth = 0;
        double totalRowsHeight = 0;
        foreach (var column in worksheet.ColumnsUsed())
        {
            totalColumnsWidth += column.Width;
        }
        foreach (var row in worksheet.RowsUsed())
        {
            totalRowsHeight += row.Height;
        }
        var scaledPictureSize = totalColumnsWidth >= totalRowsHeight ? totalRowsHeight : totalColumnsWidth;
        
        // To convert column width in pixel unit.
        double scaledPictureSizeInPixels = (scaledPictureSize * 7) + 12;
        if (scaledPictureSizeInPixels > 800)
            scaledPictureSizeInPixels = 800;

        // Place the picture with Top-Left at Cell A2
        var ixlPicture = worksheet.AddPicture(imageWatermark, "waterMark").MoveTo(worksheet.Cell("A2"));

        //Scale the image
        double scale = scaledPictureSizeInPixels / (double)ixlPicture.Width;
        ixlPicture.Scale(scale);

    }

    /// <summary>
    /// Creates an image that can be inserted as a watermark
    /// </summary>
    /// <returns>A text image</returns>
    private Stream DrawText(string text, ExcelFonts.FontName font, int fontSize, ExcelColors.Color textColor, float alpha, int rotationAngle, int height, int width)
    {
        var stream = new MemoryStream();

        //create a bitmap image with specified width and height
        var img = new Bitmap(width, height);
        var drawing = Graphics.FromImage(img);

        // Prepare string alignments
        var stringFormat = new StringFormat();
        stringFormat.Alignment = StringAlignment.Center;
        stringFormat.LineAlignment = StringAlignment.Center;

        // Create a canvas size object
        var canvasSize = new SizeF(width, height);
        // Scale to applied to the drawn text size before fitting, this will be used in practice to make soome margin space 
        var scale = 1.02;
        // Helper variable for decreasing font size if needed
        var fittedFontSize = fontSize;
        // get the size of text
        var systemFont = new Font(ExcelFonts.GetFontName(font), fittedFontSize);
        var textSize = drawing.MeasureString(text, systemFont, canvasSize, stringFormat);
        var rectWidth = (textSize.Width * scale) / 2;
        var rectHeight = (textSize.Height * scale) / 2;
        var diagonal = Math.Sqrt(Math.Pow(rectWidth, 2) + Math.Pow(rectHeight, 2));
        var diagonalAngle = Math.Atan(rectHeight / rectWidth);
        var rotationAngleRadians = rotationAngle * Math.PI / 180.0;
        var newHeight = diagonal * Math.Sin(diagonalAngle + rotationAngleRadians);
        var newWidth = diagonal * Math.Cos(diagonalAngle - rotationAngleRadians);
        
        // Adjust the canvas to make room for rotation by Reducing the font Size to fit the content inside the canvas
        // Will stop trying at the font size of one point
        while ((newHeight > height / 2.0 || newWidth / 2.0 > width) && fittedFontSize > 1)
        {
            fittedFontSize = fittedFontSize - 1;
            systemFont = new Font(ExcelFonts.GetFontName(font), fittedFontSize);
            textSize = drawing.MeasureString(text, systemFont, canvasSize, stringFormat);
            rectWidth = (textSize.Width * scale) / 2;
            rectHeight = (textSize.Height * scale) / 2;
            diagonal = Math.Sqrt(Math.Pow(rectWidth, 2) + Math.Pow(rectHeight, 2));
            diagonalAngle = Math.Atan(rectHeight / rectWidth);
            newHeight = diagonal * Math.Sin(diagonalAngle + rotationAngleRadians);
            newWidth = diagonal * Math.Cos(diagonalAngle - rotationAngleRadians);
        }

        //set rotation point
        drawing.TranslateTransform(width/2, height/2);

        //rotate the canvas
        drawing.RotateTransform(-rotationAngle);

        //reset translate transform
        drawing.TranslateTransform(-width/2, -height/2);

        //paint the background
        drawing.Clear(Color.Transparent);

        //create a brush for the text
        int alphaValue = ((int)(alpha * 255)) << 24;
        int color = ExcelColors.GetColor(textColor).Color.ToArgb();
        var textBrush = new SolidBrush(Color.FromArgb(color + alphaValue));

        //draw text on the image at center position
        drawing.DrawString(text, systemFont, textBrush, width/2, height/2, stringFormat);
        
        //Save the drawing
        drawing.Save();
        //Save to Stream in .png Format for transparency
        img.Save(stream, ImageFormat.Png);

        return stream;
    }
}


