
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace rbkApiModules.Utilities.Excel;
public interface IExcelService
{
    FileDto GenerateExcel(string Json);
}

public class ExcelService : IExcelService
{
    public FileDto GenerateExcel(string Json)
    {
        MemoryStream stream = null;

        DataModel model = ParseJson(Json);


        using (var workbook = new XLWorkbook())
        {
            var ws = workbook.Worksheets.Add("teste");

            var headersCount = model.Headers.Count;
            var linesCount = model.Results.Count;

            ws.ShowGridLines = false;

            var headerRange = "A1:" + GetExcelColumnName(headersCount) + "1";

            ws.Cells(headerRange).Value = model.Headers;

            var fullRange = "A1:" + GetExcelColumnName(headersCount) + (linesCount + 1).ToString();

            for (int i = 0; i < linesCount; i++)
            {
                for (int j = 0; j < headersCount; j++)
                {
                    var result = model.Results[i];
                    var property = Regex.Replace(model.Headers[j], "^[a-z]", m => m.Value.ToUpper());
                    var value = result.GetType().GetProperty(property).GetValue(result, null)?.ToString();

                    if (!String.IsNullOrEmpty(value) && value.Contains("<a href="))
                    {
                        result.GetType().GetProperty(property).SetValue(result, value.Replace("<a href=", "<a target=\"_blank\" href="));
                        ws.Cell(i + 2, j + 1).Value = ExtractRefFromLink(value);
                    }
                    else
                    {
                        ws.Cell(i + 2, j + 1).Value = value;
                    }
                }
            }

            for (int i = 3; i <= linesCount + 1; i = i + 2)
            {
                var rowRange = "A" + i + ":" + GetExcelColumnName(headersCount) + i;
                ws.Cells(rowRange).Style.Fill.SetPatternType(XLFillPatternValues.Solid);
                ws.Cells(rowRange).Style.Fill.SetBackgroundColor(XLColor.LightGray);
            }

            ws.Cells(headerRange).Style.Font.Bold = true;
            ws.Cells(headerRange).Style.Font.SetFontColor(XLColor.White);
            ws.Cells(headerRange).Style.Fill.SetPatternType(XLFillPatternValues.Solid);
            ws.Cells(headerRange).Style.Fill.SetBackgroundColor(XLColor.FromArgb(0, 48, 48, 48));

            ws.Cells(fullRange).Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);

            ws.Cells(headerRange).Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);

            var fullFormatingRange = "A1:" + GetExcelColumnName(headersCount - 1) + (linesCount + 1).ToString();
            ws.Cells(fullFormatingRange).Style.Border.SetRightBorder(XLBorderStyleValues.Thin);

            ws.SetAutoFilter(true);

            ws.SheetView.Freeze(2, 1);

            ws.ColumnsUsed().AdjustToContents();
            
            workbook.SaveAs(stream);
        }

        var file = new FileDto()
        {
            ContentType = "application/vnd.ms-excel",
            FileName = "teste.xlsx",
            FileStream = stream
        };

        return file;
    }

    private DataModel ParseJson(string Json)
    {
        return new DataModel();
    }

    private string GetExcelColumnName(int columnNumber)
    {
        string columnName = "";

        while (columnNumber > 0)
        {
            int modulo = (columnNumber - 1) % 26;
            columnName = Convert.ToChar('A' + modulo) + columnName;
            columnNumber = (columnNumber - modulo) / 26;
        }

        return columnName;
    }

    private string ExtractRefFromLink(string link)
    {

        if (String.IsNullOrEmpty(link))
        {
            return "";
        }

        var beginTag = "<";
        var endTag = ">";

        var value = Regex.Replace(link, $@"{beginTag}(.+?){endTag}", "");

        return value;
    }

    private class DataModel
    {
        public List<string> Headers { get; set; }
        public List<string> Results { get; set; }
    }
}



