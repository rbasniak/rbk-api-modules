using ClosedXML.Excel;
using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rbkApiModules.Utilities.Excel;

/// <summary>
/// Classe com estilos para aplicar as colunas
/// </summary>
public class ExcelStyleClasses
{
    protected ExcelStyleClasses()
    {
    }

    public ExcelStyleClasses(
        ClosedXMLDefs.ExcelFonts.FontName fontName = ClosedXMLDefs.ExcelFonts.FontName.Calibri,
        int fontSize = 11,
        bool bold = false,
        bool italic = false,
        int maxWidth = -1,
        ClosedXMLDefs.ExcelDataTypes.DataType excelDataType = ClosedXMLDefs.ExcelDataTypes.DataType.Text,
        string dataFormat = ""
        )
    {
        FontName = fontName;
        FontSize = fontSize;
        MaxWidth = maxWidth;
        ExcelDataType = excelDataType;
        DataFormat = dataFormat;
        Bold = bold;
        Italic = italic;
    }

    /// <summary>
    /// Nome da fonte para ser usada com o grupo de dados. Ex: "Calibri", "Georgia", etc
    /// </summary>
    public virtual ClosedXMLDefs.ExcelFonts.FontName FontName { get; set; }
    /// <summary>
    /// Tamanho da fonte tal como no app do excel
    /// </summary>
    public virtual int FontSize { get; set; }
    /// <summary>
    /// Tamanho da fonte tal como no app do excel
    /// </summary>
    public virtual bool Bold { get; set; }
    /// <summary>
    /// Tamanho da fonte tal como no app do excel
    /// </summary>
    public virtual bool Italic { get; set; }

    /// <summary>
    /// O tipo de dado da coluna. Ex: Number, Text, Date, TimeSpan, etc.
    /// </summary>
    public ClosedXMLDefs.ExcelDataTypes.DataType ExcelDataType { get; set; }
    /// <summary>
    /// Formatação para um tipo específico. Ex: Para o tipo Number: DataFormat = "R$ 0,00" irá formatar a celula segundo o padrão de currency.
    /// Para o tipo Date: DataFormat = "DD/MM/YYYY" irá formatar a data para o padrão utilizado no Brasil
    /// </summary>
    public virtual string DataFormat { get; set; }
    /// <summary>
    /// Se após o ajuste ao conteúdo da celula a largura deve obedecer um valor máximo. Útil para evitar que textos grandes ocupem espaços superiores ao do monitor.
    /// </summary>
    public virtual int MaxWidth { get; set; }
}
