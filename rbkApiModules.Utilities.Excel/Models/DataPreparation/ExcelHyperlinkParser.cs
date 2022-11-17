using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace rbkApiModules.Utilities.Excel;

/// <summary>
/// Helper class that parses data into dictionaries that can be stored on excel files as indexes.
/// </summary>
internal class ExcelHyperlinkParser
{
    internal void PrepareHyperlinks(ExcelColumnModel column, bool isHtml, bool isMultilined, string newLineSeparator)
    {
        if (isHtml)
        {
            if (isMultilined)
            {
                PrepareMultilinedHrefHyperlinks(column, newLineSeparator);
            }
            else
            {
                PrepareHrefHyperlinks(column);
            }
        }
        else
        {
            if (isMultilined)
            {
                PrepareMultilinedRegularHyperlinks(column, newLineSeparator);
            }
            else
            {
                PrepareRegularHyperlinks(column);
            }
        }
    }

    internal bool IsHyperlink(ExcelColumnModel column, bool isHtml)
    {
        string? linkSample;
        if (isHtml == true)
        {
            linkSample = column.Data.FirstOrDefault(x => !string.IsNullOrEmpty(x) && x.Contains("href") && x.Contains("http"), null);
        }
        else
        {
            linkSample = column.Data.FirstOrDefault(x => !string.IsNullOrEmpty(x) && x.Contains("http"), null);
        }

        if (linkSample != null)
        {
            return true;
        }

        return false;
    }

    private void PrepareMultilinedRegularHyperlinks(ExcelColumnModel column, string newLineSeparator)
    {
        column.DataType = ExcelModelDefs.ExcelDataTypes.DataType.Text;
        var data = column.Data;
        if (!String.IsNullOrEmpty(newLineSeparator))
        {
            for (int itemIndex = 0; itemIndex < data.Length; itemIndex++)
            {
                data[itemIndex] = Regex.Replace(data[itemIndex], newLineSeparator, Environment.NewLine, RegexOptions.IgnoreCase);
            }
        }
    }

    private void PrepareMultilinedHrefHyperlinks(ExcelColumnModel column, string newLineSeparator)
    {
        column.DataType = ExcelModelDefs.ExcelDataTypes.DataType.Text;
        
        var data = column.Data;

        var hasNewLineSeparator = !String.IsNullOrEmpty(newLineSeparator);

        for (int itemIndex = 0; itemIndex < data.Length; itemIndex++)
        {
            string hyperlink = data[itemIndex];

            if (hasNewLineSeparator)
            { 
                hyperlink = Regex.Replace(hyperlink, newLineSeparator, Environment.NewLine, RegexOptions.IgnoreCase);
            }

            var matches = Regex.Matches(hyperlink, @"<a.*?href=[\'""]?([^\'"" >]+).*?<\/a>", RegexOptions.IgnoreCase);

            foreach (Match match in matches)
            {
                hyperlink = hyperlink.Replace(match.Value, match.Groups[1].Value);
            }
            data[itemIndex] = hyperlink;
        }
    }

    private void PrepareRegularHyperlinks(ExcelColumnModel column)
    {
        var data = column.Data;
        var hyperlinks = new List<ExcelHyperlink>();
        for (int itemIndex = 0; itemIndex < data.Length; itemIndex++)
        {
            hyperlinks.Add(new ExcelHyperlink() { Hyperlink = data[itemIndex] });
        }
        column.AddHyperLinkData(hyperlinks.ToArray());
    }

    private void PrepareHrefHyperlinks(ExcelColumnModel column)
    {
        var data = column.Data;
        var hyperlinks = new List<ExcelHyperlink>();

        for (int itemIndex = 0; itemIndex < data.Length; itemIndex++)
        {
            if (!string.IsNullOrEmpty(data[itemIndex]))
            {
                string hyperlink = data[itemIndex];
                
                string text = Regex.Replace(hyperlink, "(</?[a|A][^>]*>|)", "");

                var matches = Regex.Matches(hyperlink, @"<a.*?href=[\'""]?([^\'"" >]+).*?<\/a>", RegexOptions.IgnoreCase);

                foreach (Match match in matches)
                {
                    hyperlink = hyperlink.Replace(match.Value, match.Groups[1].Value);
                }
                data[itemIndex] = text;
                hyperlinks.Add(new ExcelHyperlink() { Hyperlink = hyperlink });
            }
            else
            {
                hyperlinks.Add(new ExcelHyperlink() { Hyperlink = string.Empty });
            }
        }
        column.AddHyperLinkData(hyperlinks.ToArray());
    }
}
