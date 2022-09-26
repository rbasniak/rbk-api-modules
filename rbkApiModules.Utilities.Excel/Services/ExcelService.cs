using System;
using System.IO;

namespace rbkApiModules.Utilities.Excel;

public interface IExcelService
{
    ExcelsDetails GenerateSpreadsheetTablesFromWorkbookModel(ExcelWorkbookModel workbookModel);
    FileDto GenerateSpreadsheetTablesFromWorkbookModelAsFile(ExcelWorkbookModel workbookModel);
    public MemoryStream GenerateSpreadsheetTablesFromWorkbookModelAsStream(ExcelWorkbookModel workbookModel);
}

public class ExcelService : IExcelService
{
    private SaxLib _saxLib;

    public ExcelService()
    {
        _saxLib = new SaxLib();
    }

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
        var stream = _saxLib.CreatePackage(workbookModel);
        
        stream.Seek(0, SeekOrigin.Begin);
        
        return stream;  
    }
}



