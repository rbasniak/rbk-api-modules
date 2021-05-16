using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Infrastructure.Models.Charts.ChartJs.Deprecated
{ 
    public static class ChartCollorSelector
    {
        private static readonly string[] _pallete1 = new[] 
        {
            "#FFCDD2",
            "#F8BBD0",
            "#E1BEE7",
            "#D1C4E9",
            "#C5CAE9",
            "#BBDEFB",
            "#B3E5FC",
            "#B2EBF2",
            "#B2DFDB",
            "#C8E6C9",
            "#DCEDC8",
            "#F0F4C3",
            "#FFECB3",
            "#FFE0B2",
            "#FFCCBC",
            "#D7CCC8",
            "#CFD8DC",
        };

        public static string NextColor(ColorPallete pallete, int index)
        {
            switch (pallete)
            {
                case ColorPallete.Pallete1:
                    return _pallete1[index];
                default:
                    return "#777777";
            }
        }
    }

    public enum ColorPallete
    {
        Pallete1
    }
}
