using FluentValidation;
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
            .Must(HaveContinuousTabIndexCount).WithMessage("Tab Index must be continuous and start from ZERO. It cannot have skip numbers")
            // Table Type validations
            .Must(ForTableTypeHaveAtLeastOneHeaderAndOneColumn).WithMessage("The workbook model have at least one column with one header")
            .Must(ForTableTypeNotHaveRepeatedHeaderName).WithMessage("Headers inside the same spreadsheet must have unique names")
            .Must(ForTableTypeHaveNumHeadersMustEqualNumColumns).WithMessage("The number of headers must match the number of columns for each individual table")
            .Must(ForTableTypeHaveCorrectDataTypes).WithMessage("DataTypes must be between 0 and 4");
            // Future Plot Type Validations
    }

    #region Base Validations

    private bool HaveAFilename(ExcelWorkbookModel workbookModel)
    {
        return !string.IsNullOrEmpty(workbookModel.FileName);
    }

    private bool ContainAtLeastOneSheet(ExcelWorkbookModel workbookModel)
    {
        return (workbookModel.Tables != null && workbookModel.Charts != null);
    }

    private bool NotHaveEmptySheetName(ExcelWorkbookModel workbookModel)
    {
        foreach (var sheet in workbookModel.AllSheets)
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
        foreach (var sheet in workbookModel.AllSheets)
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
        foreach (var sheet in workbookModel.AllSheets)
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

    private bool HaveContinuousTabIndexCount(ExcelWorkbookModel workbookModel)
    {
        var count = 0;
        foreach (var sheet in workbookModel.AllSheets)
        {
            if (sheet.TabIndex != count)
            {
                return false;
            }
            else
            {
                count++;
            }
        }
        return true;
    }

    #endregion

    #region Table Validations

    // Table Type validations
    private bool ForTableTypeHaveAtLeastOneHeaderAndOneColumn(ExcelWorkbookModel workbookModel)
    {
        foreach (var sheet in workbookModel.Tables)
        {
            if (sheet.SheetType == ExcelModelDefs.ExcelSheetTypes.Type.Table)
            {
                if (sheet.Columns == null || sheet.Header == null)
                {
                    return false;
                }
                if (sheet.Header.Data == null)
                {
                    return false;
                }
                if (sheet.Columns.Length == 0 || sheet.Header.Data.Length == 0)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private bool ForTableTypeNotHaveRepeatedHeaderName(ExcelWorkbookModel workbookModel)
    {
        foreach (var sheet in workbookModel.Tables)
        {
            var names = new List<string>();
            if (sheet.SheetType == ExcelModelDefs.ExcelSheetTypes.Type.Table)
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

    private bool ForTableTypeHaveCorrectDataTypes(ExcelWorkbookModel workbookModel)
    {
        foreach (var sheet in workbookModel.Tables)
        {
            if (sheet.SheetType == ExcelModelDefs.ExcelSheetTypes.Type.Table)
            {
                foreach (var column in sheet.Columns)
                {
                    if (!ExcelModelDefs.ExcelDataTypes.DataType.IsDefined(column.DataType))
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    private bool ForTableTypeHaveNumHeadersMustEqualNumColumns(ExcelWorkbookModel workbookModel)
    {
        foreach (var sheet in workbookModel.Tables)
        {
            if (sheet.Header.Data.Length != sheet.Columns.Length)
            {
                return false;
            }
        }
        return true;
    }

    #endregion

    #region Chart Validations

    // Future Chart Type Validations

    #endregion
}
