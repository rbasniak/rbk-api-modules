using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Excel;

/// <summary>
/// Class describing the rules needed when auto detecting a data type on a column
/// </summary>
public class ExcelGlobalBehavior
{
    public ExcelGlobalBehavior()
    {
        Date = new ExcelDateGlobal();
        Hyperlink = new ExcelHyperlinkGlobal();
    }

    public ExcelDateGlobal Date { get; set; }
    
    public ExcelHyperlinkGlobal Hyperlink { get; set; }

    /// <summary>
    /// If a cell has multiple lines, then NewLineString must define the string which separates the lines: "\n", <br>, etc.
    /// If this is empty, then the cell doesn't have multiple lines
    /// </summary>
    public string NewLineSeparator { get; set; } = "";
}

