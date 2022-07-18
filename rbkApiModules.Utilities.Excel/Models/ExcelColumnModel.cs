using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rbkApiModules.Utilities.Excel;

/// <summary>
/// Lista de dados a serem apresentados numa coluna, abaixo dos cabeçalhos
/// </summary>
public class ExcelColumnModel
{

    /// <summary>
    /// Lista de dados a serem apresentados na coluna
    /// </summary>
    public List<string> Data { get; set; }
    /// <summary>
    /// Estilos a serem aplicados a essa coluna
    /// </summary>
    public ExcelStyleClasses Style { get; set; }
    /// <summary>
    /// Se deve aplicar o autofilter ou não
    /// </summary>
}
