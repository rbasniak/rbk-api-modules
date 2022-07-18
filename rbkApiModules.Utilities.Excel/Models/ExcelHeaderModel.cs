using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rbkApiModules.Utilities.Excel;

/// <summary>
/// Classe que representa uma linha de cabeçalho, seus dados e seus estilos.
/// </summary>
public class ExcelHeaderModel
{
    /// <summary>
    /// Lista de dados a serem apresentados na linha de cabeçalho
    /// </summary>
    public List<string> Data { get; set; }
    /// <summary>
    /// Estilos a serem aplicados a esse cabeçalho
    /// </summary>
    public ExcelStyleClasses Style { get; set; }
}