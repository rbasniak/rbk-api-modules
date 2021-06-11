using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts
{ 
    public static class ChartCollorSelector
    {
        private static readonly string[] Bright1 = new[] { "#01A5E4", "#8DD7BF", "#FF96C5", "#FF5768", "#FFBF65" };
        private static readonly string[] Bright2 = new[] { "#FC6328", "#FFD872", "#F2D4CC", "#E77577", "#6C88C4" };
        private static readonly string[] Bright3 = new[] { "#C05780", "#FF828B", "#E7C582", "#00B0BA", "#0065A2" };
        private static readonly string[] Bright4 = new[] { "#00CDAC", "#FF6F68", "#FFDACC", "#FF60A8", "#CFF800" };
        private static readonly string[] Bright5 = new[] { "#FF5C77", "#4DD091", "#FFEC59", "#FFA23A", "#74737A" };

        private static readonly string[] Spring1 = new[] { "#AFDDD5", "#FFA700", "#FFCCDD", "#F56093", "#64864A" };
        private static readonly string[] Spring2 = new[] { "#DFE6E6", "#EFDEC0", "#FF7E5A", "#FFBD00", "#7DB954" };
        private static readonly string[] Spring3 = new[] { "#FEDDCB", "#FFC700", "#CEE8E5", "#C6C598", "#FEE100" };
        private static readonly string[] Spring4 = new[] { "#FAC4C4", "#E9E7AD", "#FDBB9F", "#FFFFFF", "#EADCC3" };
        private static readonly string[] Spring5 = new[] { "#EEF3B4", "#FFB27B", "#FF284B", "#7ABAA1", "#CFEAE4" };

        private static readonly string[] Summer1 = new[] { "#53CFDA", "#EFF2E6", "#FF7994", "#FFC900", "#FFED00" };
        private static readonly string[] Summer2 = new[] { "#FF8860", "#F7D635", "#D6E8D9", "#F1C9C2", "#1F3D51" };
        private static readonly string[] Summer3 = new[] { "#FF3747", "#FF8B0F", "#FFD600", "#EAE45F", "#DDF5C2" };
        private static readonly string[] Summer4 = new[] { "#FF458F", "#FF8352", "#DEE500", "#00E1DF", "#00C3AF" };
        private static readonly string[] Summer5 = new[] { "#EDF7DD", "#4FCBBB", "#2494CC", "#EF39A7", "#FFAE90" };

        private static readonly string[] Pastel1 = new[] { "#ABDEE6", "#CBAACB", "#FFFFB5", "#FFCCB6", "#F3B0C3" };
        private static readonly string[] Pastel2 = new[] { "#FF968A", "#FFAEA5", "#FFC5BF", "#F6EAC2", "#ECD5E3" };
        private static readonly string[] Pastel3 = new[] { "#FF968A", "#FFAEA5", "#FFC5BF", "#FFD8BE", "#FFC8A2" };
        private static readonly string[] Pastel4 = new[] { "#D4F0F0", "#8FCACA", "#CCE2CB", "#B6CFB6", "#97C1A9" };

        private static readonly string[] Pastel5 = new[] { "#FCB9AA", "#FFDBCC", "#ECEAE4", "#A2E1DB", "#55CBCD" };

        private static readonly string[] Winter1 = new[] { "#445A67", "#57838D", "#B4C9C7", "#F3BFB3", "#CCADB2" };
        private static readonly string[] Winter2 = new[] { "#FFEFFF", "#F6F7FB", "#E0F8F5", "#BEEDE5", "#A7D9C9" };
        private static readonly string[] Winter3 = new[] { "#50B4D8", "#9EDDEF", "#F7E5D7", "#D7E2EA", "#96B3C2" };
        private static readonly string[] Winter4 = new[] { "#FFDAD1", "#FFEDDA", "#CAB3C1", "#6E7B8F", "#2E3332" };
        private static readonly string[] Winter5 = new[] { "#C29BA3", "#E3BFB7", "#FFE4C9", "#B7EAF7", "#8A9BA7" };

        private static readonly string[] Gemstone1 = new[] { "#53051D", "#9E1C5C", "#EF70AA", "#FF8C94", "#F12761" };
        private static readonly string[] Gemstone2 = new[] { "#00C6C7", "#96D5E2", "#00ACA5", "#006F60", "#005245" };
        private static readonly string[] Gemstone3 = new[] { "#EEAC4D", "#FFF2C3", "#EE84B3", "#740E4C", "#E2035D" };
        private static readonly string[] Gemstone4 = new[] { "#B57E79", "#FF6F68", "#00E9E7", "#006072", "#5C2A2E" };
        private static readonly string[] Gemstone5 = new[] { "#20503E", "#187B30", "#75E0B0", "#2D8498", "#4CB0A6" };

        private static readonly string[] Autumn1 = new[] { "#57291F", "#C0413B", "#D77B5F", "#FF9200", "#FFCD73" };
        private static readonly string[] Autumn2 = new[] { "#F7E5BF", "#C87505", "#F18E3F", "#E59579", "#C14C32" };
        private static readonly string[] Autumn3 = new[] { "#80003A", "#8CB5B5", "#6C3400", "#FFA400", "#EC410B" };
        private static readonly string[] Autumn4 = new[] { "#E63400", "#8CB5B5", "#6C3400", "#FFA400", "#41222A" };
        private static readonly string[] Autumn5 = new[] { "#FFF7C2", "#FFB27B", "#FF7B80", "#BC7576", "#696B7E" };

        private static readonly string[] Vivid1 = new[] { "#FF3784", "#36A2EB", "#4BC0C0", "#F77825", "#9966FF" };
        private static readonly string[] Vivid2 = new[] { "#0074D9", "#FF4136", "#2ECC40", "#FF851B", "#7FDBFF" };
        private static readonly string[] Vivid3 = new[] { "#2D95EC", "#F6BA2A", "#F64D2A", "#8ABB21", "#E2711D" };
        private static readonly string[] Vivid4 = new[] { "#EC1F24", "#69BC45", "#3651A2", "#F3EC1F", "#6CCCDE" };
        private static readonly string[] Vivid5 = new[] { "#EC0F7B", "#F6831B", "#97C93B", "#6CC175", "#6950A1" };
        private static readonly string[] Vivid6 = new[] { "#CD1724", "#07B8FF", "#34A300", "#8D4D00", "#FFE700" };

        private static readonly string[] General1 = new[] { "#4573A7", "#AA4644", "#89A54E", "#71588F", "#4298AF" };
        private static readonly string[] General2 = new[] { "#DB843D", "#93A9D0", "#D09392", "#BACD96", "#A99BBE" };
        private static readonly string[] General3 = new[] { "#68D4CD", "#CFF67B", "#94DAFB", "#FD8080", "#6C838D" };
        private static readonly string[] General4 = new[] { "#26A0FC", "#26E7A6", "#FEBC3B", "#FAB1B2", "#8B75D7" };
        private static readonly string[] General5 = new[] { "#E64345", "#E48F1B", "#F7D027", "#6BA547", "#619ED6" };
        private static readonly string[] General6 = new[] { "#60CEED", "#9CF168", "#F7EA4A", "#FBC543", "#FFC9ED" };
        private static readonly string[] General7 = new[] { "#64D0DA", "#34B2E4", "#065381", "#8B103E", "#E34856" };
        private static readonly string[] General8 = new[] { "#FE912A", "#E6696E", "#B64D8B", "#554D89", "#003E59" };

        private static readonly string[] Red1 = new[] { "#4F2623", "#632724", "#923734", "#A93F3B", "#BA5754" };
        private static readonly string[] Red2 = new[] { "#D13A2C", "#E14A3B", "#F0594B", "#F76959", "#F7C6B5" };

        private static readonly string[] Blue1 = new[] { "#2244A1", "#345DB3", "#537BD2", "#6A94DD", "#9DC2F7" };
        private static readonly string[] Blue2 = new[] { "#2082CA", "#3797D8", "#44ABE8", "#58C5EF", "#78D8F5" };

        private static readonly string[] Green1 = new[] { "#39590A", "#51761C", "#689E2C", "#91C637", "#A2D970" };
        private static readonly string[] Green2 = new[] { "#005F03", "#0F8D00", "#2EC500", "#48DE48", "#9FF3A1" };
        private static readonly string[] Green3 = new[] { "#CEC358", "#D7C819", "#E9DA1E", "#F0E785", "#F5F1B7" };
        private static readonly string[] Green4 = new[] { "#0D754E", "#10A86F", "#46B98C", "#76CBA8", "#BBE4D4" };

        private static readonly string[] Yellow1 = new[] { "#A07900", "#C69A12", "#E5B72B", "#F1C04C", "#F1D185" };

        private static readonly string[] Pink1 = new[] { "#C75B73", "#DA7B8E", "#E79099", "#E4A2A7", "#ECBCBC" };
        private static readonly string[] Pink2 = new[] { "#F73E3E", "#F75D5D", "#F77C7C", "#F79B9B", "#F7BABA" };

        private static readonly string[] Soft1 = new[] { "#FFCDD2", "#F8BBD0", "#E1BEE7", "#D1C4E9", "#C5CAE9" };
        private static readonly string[] Soft2 = new[] { "#BBDEFB", "#B3E5FC", "#B2EBF2", "#B2DFDB", "#C8E6C9" };
        private static readonly string[] Soft3 = new[] { "#DCEDC8", "#F0F4C3", "#FFECB3", "#FFE0B2", "#FFCCBC" };

        internal static string[] GetColors(params ColorPallete[] palletes)
        {
            if (palletes.Length == 0) throw new ArgumentException("You need to specify ate least one color pallete");

            var colors = new List<string>();

            foreach (var pallete in palletes)
            {
                var type = typeof(ChartCollorSelector);
                var info = type.GetField(pallete.ToString(), BindingFlags.NonPublic | BindingFlags.Static);
                var value = (string[])info.GetValue(null);

                colors.AddRange(value);
            }

            return colors.ToArray();
        }
    }

    public enum ColorPallete
    {
        Bright1 = 0,
        Bright2 = 1,
        Bright3 = 2,
        Bright4 = 3,
        Bright5 = 4,

        Spring1 = 5,
        Spring2 = 6,
        Spring3 = 7,
        Spring4 = 8,
        Spring5 = 9,

        Summer1 = 10,
        Summer2 = 11,
        Summer3 = 12,
        Summer4 = 13,
        Summer5 = 14,

        Pastel1 = 15,
        Pastel2 = 16,
        Pastel3 = 17,
        Pastel4 = 18,
        Pastel5 = 19,

        Winter1 = 20,
        Winter2 = 21,
        Winter3 = 22,
        Winter4 = 23,
        Winter5 = 24,

        Gemstone1 = 25,
        Gemstone2 = 26,
        Gemstone3 = 27,
        Gemstone4 = 28,
        Gemstone5 = 29,

        Autumn1 = 30,
        Autumn2 = 31,
        Autumn3 = 32,
        Autumn4 = 33,
        Autumn5 = 34,

        Vivid1 = 35,
        Vivid2 = 36,
        Vivid3 = 37,
        Vivid4 = 38,
        Vivid5 = 39,
        Vivid6 = 40,

        General1 = 41,
        General2 = 42,
        General3 = 43,
        General4 = 44,
        General5 = 45,
        General6 = 46,
        General7 = 47,
        General8 = 48,

        Red1 = 49,
        Red2 = 50,

        Blue1 = 51,
        Blue2 = 52,

        Green1 = 53,
        Green2 = 54,
        Green3 = 55,
        Green4 = 56,

        Yellow1 = 57,

        Pink1 = 58,
        Pink2 = 59,

        Soft1 = 60,
        Soft2 = 61,
        Soft3 = 62
    }
}
