using FluentValidation;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;


namespace rbkApiModules.Utilities.Excel;

public interface IExcelWorkbookModelJSonValidator
{
    public ExcelWorkbookModel WorkbookModel { get; set; }
}

public class ExcelWorkbookModelJSonValidator : AbstractValidator<IExcelWorkbookModelJSonValidator>
{
    public ExcelWorkbookModelJSonValidator()
    {

        RuleFor(x => x.WorkbookModel)
        .NotNull().WithMessage("JSON File can't be null.")
        .Must(HaveAFilename).WithMessage("The workbook model must have a filename")
        .Must(ContainAtLeastOneSheet).WithMessage("The workbook model must contain at least one spreadsheet")
        .Must(NotHaveEmptySheetName).WithMessage("Sheet Name cannot be empty")
        .Must(NotHaveSheetNameGreaterThenThirtyOneChars).WithMessage("Sheet Name cannot have more than 31 characters")
        .Must(NotHaveRepeatedSheetNames).WithMessage("Workbook models containing more than one sheet must have unique sheet names for the tabs")
        // Table Type validations
        .Must(ForTableTypeHaveAtLeastOndeHeaderAndOneColumn).WithMessage("The workbook model have at least one column with one header")
        .Must(ForTableTypeNotHaveRepeatedHeaderName).WithMessage("Headers inside the same spreadsheet must have unique names");
        // Future Plot Type Validations
    }

    #region Base Validations

    private bool HaveAFilename(ExcelWorkbookModel workbookModel)
    {
        return !string.IsNullOrEmpty(workbookModel.FileName);
    }

    private bool ContainAtLeastOneSheet(ExcelWorkbookModel workbookModel)
    {
        return workbookModel.Sheets != null;
    }

    private bool NotHaveEmptySheetName(ExcelWorkbookModel workbookModel)
    {
        foreach (var sheet in workbookModel.Sheets)
        {
            if (string.IsNullOrEmpty(sheet.Name))
            {
                return false;
            }
        }
        return true;
    }

    private bool NotHaveSheetNameGreaterThenThirtyOneChars(ExcelWorkbookModel workbookModel)
    {
        foreach (var sheet in workbookModel.Sheets)
        {
            if (sheet.Name.Length > 31)
            {
                return false;
            }
        }
        return true;
    }

    private bool NotHaveRepeatedSheetNames(ExcelWorkbookModel workbookModel)
    {
        var names = new List<string>();
        foreach (var sheet in workbookModel.Sheets)
        {
            if (names.Contains(sheet.Name.ToLower()))
            {
                return false;
            }
            else
            {
                names.Add(sheet.Name.ToLower());
            }
        }
        return true;
    }

    #endregion

    #region Table Validations

    // Table Type validations
    private bool ForTableTypeHaveAtLeastOndeHeaderAndOneColumn(ExcelWorkbookModel workbookModel)
    {
        foreach (var sheet in workbookModel.Sheets)
        {
            if (sheet.SheetType == ClosedXMLDefs.ExcelSheetTypes.Type.Table)
            {
                var tableSheet = sheet as ExcelSheetTableModel;
                if (tableSheet.Columns == null || tableSheet.Header == null)
                {
                    return false;
                }
                if (tableSheet.Header.Data == null)
                {
                    return false;
                }
                if (tableSheet.Columns.Length == 0 || tableSheet.Header.Data.Length == 0)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private bool ForTableTypeNotHaveRepeatedHeaderName(ExcelWorkbookModel workbookModel)
    {
        foreach (var sheet in workbookModel.Sheets)
        {
            var names = new List<string>();
            if (sheet.SheetType == ClosedXMLDefs.ExcelSheetTypes.Type.Table)
            {
                foreach (var headerName in sheet.Header.Data)
                {
                    if (names.Contains(headerName.ToLower()))
                    {
                        return false;
                    }
                    else
                    {
                        names.Add(headerName.ToLower());
                    }
                }
            }
        }
        return true;
    }

    #endregion

    #region Plot Validations

    // Future Plot Type Validations

    #endregion
}
