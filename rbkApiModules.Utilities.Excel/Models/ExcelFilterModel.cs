using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rbkApiModules.Utilities.Excel;

/// <summary>
/// Classe para definir filtros para colunas
/// </summary>
public class FilterModel
{
    /// <summary>
    /// Tipos de filtros: Contains, Before, After, etc.
    /// </summary>
    public string FilterTypes { get; set; } = "";
    /// <summary>
    /// Query de filtro
    /// </summary>
    public string FilterData { get; set; } = "";
}
