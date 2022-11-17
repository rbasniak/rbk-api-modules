using System;
using System.IO;

namespace rbkApiModules.Utilities.Excel;

public interface IExcelService
{
    SpreadsheetDetails GenerateSpreadsheetAsBase64(ExcelWorkbookModel workbookModel);
    FileData GenerateSpreadsheetAsStream(ExcelWorkbookModel workbookModel);
    MemoryStream GenerateSpreadsheet(ExcelWorkbookModel workbookModel);
}

public class ExcelService : IExcelService
{
    private SaxLib _saxLib;

    public ExcelService()
    {
        _saxLib = new SaxLib();
    }

    public FileData GenerateSpreadsheetAsStream(ExcelWorkbookModel workbookModel)
    {
        var stream = GenerateSpreadsheet(workbookModel);
        // Create the FileDto to be returned as stream to the client
        var file = new FileData()
        {
            FileName = workbookModel.FileName + ".xlsx",
            ContentType = "application/vnd.ms-excel",
            FileStream = stream
        };
    
        return file;
    }
    public SpreadsheetDetails GenerateSpreadsheetAsBase64(ExcelWorkbookModel workbookModel)
    {
        var stream = GenerateSpreadsheet(workbookModel);

        // Create the FileDto to be returned as stream to the client
        var file = new SpreadsheetDetails()
        {
            FileName = workbookModel.FileName,
            FileExtension = "xlsx",
            File = Convert.ToBase64String(stream.ToArray())
        };
        
        return file;
    }

    public MemoryStream GenerateSpreadsheet(ExcelWorkbookModel workbookModel)
    {
        var stream = _saxLib.CreatePackage(workbookModel);
        
        stream.Seek(0, SeekOrigin.Begin);
        
        return stream;  
    }
}



