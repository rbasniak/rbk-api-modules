using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Avatar
{
    public static class AvatarGenerator
    {
        private static List<string> _colors = new List<string>();
        private static Random _random = new Random();

        static AvatarGenerator()
        {
            _colors.Add("#b284be");
            _colors.Add("#9966cc");
            _colors.Add("#8a2be2");
            _colors.Add("#702963");
            _colors.Add("#856088");
            _colors.Add("#551a8b");
            _colors.Add("#9400d3");
            _colors.Add("#6f00ff");
            _colors.Add("#bf00ff");
            _colors.Add("#563c5c");
            _colors.Add("#6f2da8");
            _colors.Add("#df73ff");
            _colors.Add("#4B0082");
            _colors.Add("#6a5acd");
            _colors.Add("#daa520");
            _colors.Add("#800000");
            _colors.Add("#800020");
            _colors.Add("#b38b6d");
            _colors.Add("#d2b48c");
            _colors.Add("#cd7f32");
            _colors.Add("#b87333");
            _colors.Add("#e97451");
            _colors.Add("#bdb76b");
            _colors.Add("#808000");
            _colors.Add("#a0522d");
            _colors.Add("#8a3324");
            _colors.Add("#954535");
            _colors.Add("#c2b280");
            _colors.Add("#cc7722");
            _colors.Add("#40e0d0");
            _colors.Add("#4169e1");
            _colors.Add("#000080");
            _colors.Add("#0047ab");
            _colors.Add("#add8e6");
            _colors.Add("#89CFF0");
            _colors.Add("#0abab5");
            _colors.Add("#00bfff");
            _colors.Add("#003153");
            _colors.Add("#0892d0");
            _colors.Add("#00008b");
            _colors.Add("#003366");
            _colors.Add("#0073cf");
            _colors.Add("#2a52be");
            _colors.Add("#6495ed");
            _colors.Add("#008080");
            _colors.Add("#40E0D0");
            _colors.Add("#7fff00");
            _colors.Add("#4cbb17");
            _colors.Add("#228b22");
            _colors.Add("#32cd32");
            _colors.Add("#355e3b");
            _colors.Add("#006400");
            _colors.Add("#7fffd4");
            _colors.Add("#aaf0d1");
            _colors.Add("#90EE90");
            _colors.Add("#00ff7f");
            _colors.Add("#77dd77");
            _colors.Add("#f5f5dc");
            _colors.Add("#fff44f");
            _colors.Add("#ffef00");
            _colors.Add("#ffdf00");
            _colors.Add("#f0e130");
            _colors.Add("#e4d96f");
            _colors.Add("#e4d00a");
            _colors.Add("#b5a642");
            _colors.Add("#9acd32");
            _colors.Add("#808000");
            _colors.Add("#eedc82");
            _colors.Add("#e1ad01");
            _colors.Add("#c2b280");
            _colors.Add("#f4c430");
            _colors.Add("#b38b6d");
            _colors.Add("#C0C0C0");
            _colors.Add("#36454f");
            _colors.Add("#778899");
            _colors.Add("#6699cc");
            _colors.Add("#2C3539");
            _colors.Add("#b2beb5");
            _colors.Add("#a7a6ba");
            _colors.Add("#848482");
            _colors.Add("#8c92ac");
            _colors.Add("#f7cac9");
            _colors.Add("#40404F");
            _colors.Add("#dbd7d2");
            _colors.Add("#8f8b66");
            _colors.Add("#551a8b");
            _colors.Add("#282C35");
            _colors.Add("#36454f");
            _colors.Add("#0f0f0f");
            _colors.Add("#003366");
            _colors.Add("#3b3c36");
            _colors.Add("#002147");
            _colors.Add("#343434");
            _colors.Add("#612302");
            _colors.Add("#100c08");
            _colors.Add("#3d2b1f");
            _colors.Add("#1a1110");
            _colors.Add("#16161D");
            _colors.Add("#3d0c02");
            _colors.Add("#32174d");
            _colors.Add("#f88379");
            _colors.Add("#ff69b4");
            _colors.Add("#800000");
            _colors.Add("#800020");
            _colors.Add("#922724");
            _colors.Add("#FF00FF");
            _colors.Add("#FF0000");
            _colors.Add("#ca2c92");
            _colors.Add("#8b0000");
            _colors.Add("#c04000");
            _colors.Add("#8A0707");
            _colors.Add("#ff2400");
            _colors.Add("#66023c");
            _colors.Add("#b7410e");
            _colors.Add("#fbceb1");
            _colors.Add("#E6E6FA");
            _colors.Add("#ffe5b4");
            _colors.Add("#de5d83");
            _colors.Add("#ffdead");
            _colors.Add("#eedc82");
            _colors.Add("#f1e1cc");
            _colors.Add("#fff44f");
            _colors.Add("#fbceb1");
            _colors.Add("#FF6700");
            _colors.Add("#D4AF37");
            _colors.Add("#f0dc82");
            _colors.Add("#cd5700");
            _colors.Add("#ee7600");
            _colors.Add("#ffb347");
            _colors.Add("#f28500");
            _colors.Add("#ffa500");
            _colors.Add("#e24c00");
            _colors.Add("#EC5800");
            _colors.Add("#ff4f00");
            _colors.Add("#ff4500");
            _colors.Add("#eba832");
            _colors.Add("#fbceb1");
            _colors.Add("#ff77ff");
            _colors.Add("#f88379");
            _colors.Add("#a94064");
            _colors.Add("#fc8eac");
            _colors.Add("#e75480");
            _colors.Add("#f4c2c2");
            _colors.Add("#ff9999");
            _colors.Add("#ffe5b4");
            _colors.Add("#fc6c85");
            _colors.Add("#ff00ff");
            _colors.Add("#ff1493");
            _colors.Add("#ffd1dc");
            _colors.Add("#ffb6c1");
            _colors.Add("#65000b");
            _colors.Add("#ff69b4");
        }

        public static string GenerateSvgAvatar(string fullName)
        {
            var parts = fullName.Split(' ');

            var initials = null;

            if (parts == 1)
            {
                initials = parts.First().ToUpper();
            }
            else
            {
                initials = parts.First().ToUpper() + parts.Last().ToUpper();
            }

            var svg = "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"256px\" height=\"256px\"><rect width=\"256\" height=\"256\" style=\"fill:{{COLOR}}\"/><text x=\"50%\" y=\"50%\" dominant-baseline=\"middle\" text-anchor=\"middle\" style=\"font-size:160px;fill:#fff;font-family:ArialMT, Arial\">{{INITIALS}}</text></svg>";

            svg = svg.Replace("{{COLOR}}", _colors[_random.Next(0, _colors.Count)]).Replace("{{INITIALS}}", initials);

            return svg;
        }

        public static string GenerateBase64Avatar(string fullName)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(GenerateSvgAvatar(fullName));
            return "data:image/svg+xml;base64," + System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
