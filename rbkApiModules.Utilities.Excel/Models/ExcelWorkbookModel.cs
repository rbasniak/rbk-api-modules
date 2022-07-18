using ClosedXML.Excel;
using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rbkApiModules.Utilities.Excel;

public class ExcelWorkbookModel
{
    private HashSet<ExcelSheetModel> _sheets;

    public ExcelWorkbookModel(
        ClosedXMLDefs.ExcelThemes.Theme theme = ClosedXMLDefs.ExcelThemes.Theme.None,
        string author = "",
        string company = "",
        string dateCreated = "",
        string comments = ""
        )
    {
        _sheets = new HashSet<ExcelSheetModel>();
        Author = author;
        Company = company;
        DateCreated = dateCreated;
        Comments = comments;
    }

    /// <summary>
    /// Caso diferente de "None" esta opção vincula o tema da tabela a um preset do excel
    /// </summary>
    public virtual ClosedXMLDefs.ExcelThemes.Theme TableTheme { get; set; }
    /// <summary>
    /// Metadado de autoria,nome do autor da planilha
    /// </summary>
    public virtual string Author { get; set; }
    /// <summary>
    /// Metadado de autoria, nome da companhia responsável
    /// </summary>
    public virtual string Company {get; set;}
    /// <summary>
    /// Metadado de autoria, data da criação da planilha
    /// </summary>
    public virtual string DateCreated {get; set;}
    /// <summary>
    /// Metadado de autoria, comentários sobre a planilha
    /// </summary>
    public virtual string Comments { get; set; } 
    /// <summary>
    /// Lista com todas as planilhas dentro do workbook excel
    /// </summary>
    public virtual IEnumerable<ExcelSheetModel> Sheets => _sheets?.ToList();
}
