using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rbkApiModules.Utilities.Excel;

/// <summary>
/// Classe que guarda os dados de uma única planilha
/// </summary>
public class ExcelSheetModel
{
    private HashSet<ExcelColumnModel> _columns;

    protected ExcelSheetModel()
    {
    }

    public ExcelSheetModel(
        string name,
        bool shouldSort = false,
        int sortColumn = 1,
        bool matchCase = false,
        bool ignoreBlanks = false,
        ClosedXMLDefs.ExcelSort.SortOrder sortOrder = ClosedXMLDefs.ExcelSort.SortOrder.Ascending)
    {
        _columns = new HashSet<ExcelColumnModel>();
        Name = name;
        ShouldSort = shouldSort;
        SortColumn = sortColumn;
        MatchCase = matchCase;
        IgnoreBlanks = ignoreBlanks;
    }

    /// <summary>
    /// Nome da planilha
    /// </summary>
    public virtual string Name { get; set; }
    /// <summary>
    /// Se a planilha será ordenada
    /// </summary>
    public bool ShouldSort { get; set; }
    /// <summary>
    /// Caso seja ordenada, se a ordenação deve diferenicar maiúsculas de minúsculas
    /// </summary>
    public bool MatchCase { get; set; }
    /// <summary>
    /// Caso seja ordenada, se a ordenação deve ignorar espaços em branco
    /// </summary>
    public bool IgnoreBlanks { get; set; }
    /// <summary>
    /// Caso seja ordenada, a coluna que irá guiar a ordenação
    /// </summary>
    public int SortColumn { get; set; }
    /// <summary>
    /// Caso seja ordenada, se a ordenação será ascendente ou descendente
    /// </summary>
    public ClosedXMLDefs.ExcelSort.SortOrder SortOrder { get; set; }
    /// <summary>
    /// Lista de dados de cabeçalho da planilha 
    /// </summary>
    public virtual ExcelHeaderModel Header { get; set; }
    /// <summary>
    /// Lista de dados da planilha
    /// </summary>
    public virtual IEnumerable<ExcelColumnModel> Columns => _columns?.ToList();
}
