using System;
using System.Drawing;

namespace rbkApiModules.Utilities.Excel;

public class Watermark
{
    /// <summary>
    /// Se a planilha é um rascunho
    /// </summary>
    public virtual bool IsDraft { get; set; } = false;
    /// <summary>
    /// Caso seja rascunho, o texto a ser inserido na marca d'água
    /// </summary>
    public virtual string Text { get; set; } = "";
    /// <summary>
    /// A transparencia da marca d'agua
    /// </summary>
    public virtual int Alpha { get; set; } = 0;
    /// <summary>
    /// O nome da cor a ser utilizada
    /// </summary>
    public string TextColor { get; set; } = "Black";
    /// <summary>
    /// A fonte que será usada para cria a marca d'água
    /// </summary>
    public virtual ClosedXMLDefs.ExcelFonts.FontName FontName { get; set; } = ClosedXMLDefs.ExcelFonts.FontName.Calibri;
    /// <summary>
    /// Angle to rotate the watermark
    /// </summary>
    public virtual int RotationAngle { get; set; } = 0;
    /// <summary>
    /// O tamanho da fonte para utilizar com a marca d'água
    /// </summary>
    public virtual int FontSize { get; set; } = 40;
    /// <summary>
    /// Altura da marca d'água não rotacionada
    /// </summary>
    public virtual double Height { get; set; }
    /// <summary>
    /// Largura da marca d'água não rotacionada
    /// </summary>
    public virtual double Width { get; set; }
    /// <summary>
    /// Cria uma marca d'água rotacionada de 45 graus
    /// </summary>
    /// <returns>A imagem para ser inserida como marca d'água na planilha</returns>
    public Image DrawText()

    {
        //create a bitmap image with specified width and height
        Image img = new Bitmap((int)Width, (int)Height);

        Graphics drawing = Graphics.FromImage(img);

        //get the size of text
        var font = new Font(ClosedXMLDefs.ExcelFonts.GetFontName((int)FontName), FontSize);
        SizeF textSize = drawing.MeasureString(Text, font);

        //set rotation point
        drawing.TranslateTransform(((int)Width - textSize.Width) / 2, ((int)Height - textSize.Height) / 2);

        //rotate text
        drawing.RotateTransform(-45);

        //reset translate transform
        drawing.TranslateTransform(-((int)Width - textSize.Width) / 2, -((int)Height - textSize.Height) / 2);

        //paint the background
        drawing.Clear(Color.Transparent);

        //create a brush for the text
        var color = Color.FromName(TextColor).ToArgb() + Alpha<<24;
        Brush textBrush = new SolidBrush(Color.FromArgb(color));

        //draw text on the image at center position
        drawing.DrawString(Text, font, textBrush, ((int)Width - textSize.Width) / 2, ((int)Height - textSize.Height) / 2);

        drawing.Save();

        return img;
    }

}
