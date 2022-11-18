using System;

namespace rbkApiModules.Utilities.Excel;

public static class ExcelModelDefs
{
    public static class Configuration
    {
        public const int NumLengthSamples = 50;
        public const string ColorPattern = @"^([A-Fa-f0-9]{8})$";
    }

    public static class StyleContants
    {
        public static UInt32 StartIndex
        {
            get
            {
                return 164;
            }
        }
    }

    public static class ExcelSheetTypes
    {
        public enum Type
        {
            Table = 0,
            Chart = 1
        }
    }

    public static class ExcelThemes
    {
        public enum Theme
        {
            None = 0,
            TableStyleLight1 = 1,
            TableStyleLight2 = 2,
            TableStyleLight3 = 3,
            TableStyleLight4 = 4,
            TableStyleLight5 = 5,
            TableStyleLight6 = 6,
            TableStyleLight7 = 7,
            TableStyleLight8 = 8,
            TableStyleLight9 = 9,
            TableStyleLight10 = 10,
            TableStyleLight11 = 11,
            TableStyleLight12 = 12,
            TableStyleLight13 = 13,
            TableStyleLight14 = 14,
            TableStyleLight15 = 15,
            TableStyleLight16 = 16,
            TableStyleLight17 = 17,
            TableStyleLight18 = 18,
            TableStyleLight19 = 19,
            TableStyleLight20 = 20,
            TableStyleLight21 = 21,
            TableStyleMedium1 = 22,
            TableStyleMedium2 = 23,
            TableStyleMedium3 = 24,
            TableStyleMedium4 = 25,
            TableStyleMedium5 = 26,
            TableStyleMedium6 = 27,
            TableStyleMedium7 = 28,
            TableStyleMedium8 = 29,
            TableStyleMedium9 = 30,
            TableStyleMedium10 = 31,
            TableStyleMedium11 = 32,
            TableStyleMedium12 = 33,
            TableStyleMedium13 = 34,
            TableStyleMedium14 = 35,
            TableStyleMedium15 = 36,
            TableStyleMedium16 = 37,
            TableStyleMedium17 = 38,
            TableStyleMedium18 = 39,
            TableStyleMedium19 = 40,
            TableStyleMedium20 = 41,
            TableStyleMedium21 = 42,
            TableStyleDark1 = 43,
            TableStyleDark2 = 44,
            TableStyleDark3 = 45,
            TableStyleDark4 = 46,
            TableStyleDark5 = 47,
            TableStyleDark6 = 48,
            TableStyleDark7 = 49,
            TableStyleDark8 = 50,
            TableStyleDark9 = 51,
            TableStyleDark10 = 52,
            TableStyleDark11 = 53
        }

        public static string GetTheme(Theme theme)
        {
            switch ((int)theme)
            {
                case 0: return "None";
                case 1: return "TableStyleLight1";
                case 2: return "TableStyleLight2";
                case 3: return "TableStyleLight3";
                case 4: return "TableStyleLight4";
                case 5: return "TableStyleLight5";
                case 6: return "TableStyleLight6";
                case 7: return "TableStyleLight7";
                case 8: return "TableStyleLight8";
                case 9: return "TableStyleLight9";
                case 10: return "TableStyleLight10";
                case 11: return "TableStyleLight11";
                case 12: return "TableStyleLight12";
                case 13: return "TableStyleLight13";
                case 14: return "TableStyleLight14";
                case 15: return "TableStyleLight15";
                case 16: return "TableStyleLight16";
                case 17: return "TableStyleLight17";
                case 18: return "TableStyleLight18";
                case 19: return "TableStyleLight19";
                case 20: return "TableStyleLight20";
                case 21: return "TableStyleLight21";
                case 22: return "TableStyleMedium1";
                case 23: return "TableStyleMedium2";
                case 24: return "TableStyleMedium3";
                case 25: return "TableStyleMedium4";
                case 26: return "TableStyleMedium5";
                case 27: return "TableStyleMedium6";
                case 28: return "TableStyleMedium7";
                case 29: return "TableStyleMedium8";
                case 30: return "TableStyleMedium9";
                case 31: return "TableStyleMedium10";
                case 32: return "TableStyleMedium11";
                case 33: return "TableStyleMedium12";
                case 34: return "TableStyleMedium13";
                case 35: return "TableStyleMedium14";
                case 36: return "TableStyleMedium15";
                case 37: return "TableStyleMedium16";
                case 38: return "TableStyleMedium17";
                case 39: return "TableStyleMedium18";
                case 40: return "TableStyleMedium19";
                case 41: return "TableStyleMedium20";
                case 42: return "TableStyleMedium21";
                case 43: return "TableStyleDark1";
                case 44: return "TableStyleDark2";
                case 45: return "TableStyleDark3";
                case 46: return "TableStyleDark4";
                case 47: return "TableStyleDark5";
                case 48: return "TableStyleDark6";
                case 49: return "TableStyleDark7";
                case 50: return "TableStyleDark8";
                case 51: return "TableStyleDark9";
                case 52: return "TableStyleDark10";
                case 53: return "TableStyleDark11";
                default:
                    return "None";
            }
        }
    }

    public static class ExcelFonts
    {
        public enum FontType
        {
            Arial = 0,
            Calibri = 1,
            CalibriLight = 2,
            CourierNew = 3,
            TimesNewRoman = 4
        }

        public static double GetFontSizeFactor(FontType fontType)
        {
            switch ((int)fontType)
            {
                case 0: return 10.5D;
                case 1: return 10D;
                case 2: return 10D;
                case 3: return 11D;
                case 4: return 11D;
                default: return 11D;
            }
        }
    }

    public static class ExcelDataTypes
    {
        public enum DataType
        {
            Text = 0,
            Number = 1,
            DateTime = 2,
            HyperLink = 3,
            AutoDetect = 4
        }
    }

    public static class ExcelSort
    {
        public enum SortOrder
        {
            Ascending = 0,
            Descending = 1
        }
    }
    
}
