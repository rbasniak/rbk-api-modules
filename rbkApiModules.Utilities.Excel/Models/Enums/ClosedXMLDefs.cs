using ClosedXML.Excel;

namespace rbkApiModules.Utilities.Excel;

public static class ClosedXMLDefs
{
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

        public static XLTableTheme GetTheme(Theme theme)
        {
            switch ((int)theme)
            {
                case 1: return XLTableTheme.TableStyleLight1;
                case 2: return XLTableTheme.TableStyleLight2;
                case 3: return XLTableTheme.TableStyleLight3;
                case 4: return XLTableTheme.TableStyleLight4;
                case 5: return XLTableTheme.TableStyleLight5;
                case 6: return XLTableTheme.TableStyleLight6;
                case 7: return XLTableTheme.TableStyleLight7;
                case 8: return XLTableTheme.TableStyleLight8;
                case 9: return XLTableTheme.TableStyleLight9;
                case 10: return XLTableTheme.TableStyleLight10;
                case 11: return XLTableTheme.TableStyleLight11;
                case 12: return XLTableTheme.TableStyleLight12;
                case 13: return XLTableTheme.TableStyleLight13;
                case 14: return XLTableTheme.TableStyleLight14;
                case 15: return XLTableTheme.TableStyleLight15;
                case 16: return XLTableTheme.TableStyleLight16;
                case 17: return XLTableTheme.TableStyleLight17;
                case 18: return XLTableTheme.TableStyleLight18;
                case 19: return XLTableTheme.TableStyleLight19;
                case 20: return XLTableTheme.TableStyleLight20;
                case 21: return XLTableTheme.TableStyleLight21;
                case 22: return XLTableTheme.TableStyleMedium1;
                case 23: return XLTableTheme.TableStyleMedium2;
                case 24: return XLTableTheme.TableStyleMedium3;
                case 25: return XLTableTheme.TableStyleMedium4;
                case 26: return XLTableTheme.TableStyleMedium5;
                case 27: return XLTableTheme.TableStyleMedium6;
                case 28: return XLTableTheme.TableStyleMedium7;
                case 29: return XLTableTheme.TableStyleMedium8;
                case 30: return XLTableTheme.TableStyleMedium9;
                case 31: return XLTableTheme.TableStyleMedium10;
                case 32: return XLTableTheme.TableStyleMedium11;
                case 33: return XLTableTheme.TableStyleMedium12;
                case 34: return XLTableTheme.TableStyleMedium13;
                case 35: return XLTableTheme.TableStyleMedium14;
                case 36: return XLTableTheme.TableStyleMedium15;
                case 37: return XLTableTheme.TableStyleMedium16;
                case 38: return XLTableTheme.TableStyleMedium17;
                case 39: return XLTableTheme.TableStyleMedium18;
                case 40: return XLTableTheme.TableStyleMedium19;
                case 41: return XLTableTheme.TableStyleMedium20;
                case 42: return XLTableTheme.TableStyleMedium21;
                case 43: return XLTableTheme.TableStyleDark1;
                case 44: return XLTableTheme.TableStyleDark2;
                case 45: return XLTableTheme.TableStyleDark3;
                case 46: return XLTableTheme.TableStyleDark4;
                case 47: return XLTableTheme.TableStyleDark5;
                case 48: return XLTableTheme.TableStyleDark6;
                case 49: return XLTableTheme.TableStyleDark7;
                case 50: return XLTableTheme.TableStyleDark8;
                case 51: return XLTableTheme.TableStyleDark9;
                case 52: return XLTableTheme.TableStyleDark10;
                case 53: return XLTableTheme.TableStyleDark11;
                case 0:
                default:
                    return XLTableTheme.None;
            }
        }
    }

    public static class ExcelFonts
    {
        public enum FontName
        {
            Arial = 0,
            ArialBold = 1,
            ArialNarrow = 2,
            Calibri = 3,
            CalibriLight = 4,
            CourierNew = 5,
            TimesNewRoman = 6,
            Georgia = 7
        }

        public static string GetFontName(FontName font)
        {
            switch((int)font)
            {
                case 0: return "Arial";
                case 1: return "Arial Bold";
                case 2: return "Arial Narrow";
                case 4: return "Calibri Light";
                case 5: return "Courrier New";
                case 6: return "Times New Roman";
                case 7: return "Georgia";
                case 8: return "Georgia Pro";
                case 3:
                default:
                    return "Calibri";
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
            Boolean = 3,
            TimeSpan = 4
        }

        public static XLDataType GetDataType(DataType type)
        {
            switch((int)type)
            {
                case 1: return XLDataType.Number;
                case 2: return XLDataType.DateTime;
                case 3: return XLDataType.Boolean;
                case 4: return XLDataType.TimeSpan;
                case 0:
                default:
                    return XLDataType.Text;
            }
        }
    }

    public static class ExcelSort
    {
        public enum SortOrder
        {
            Ascending = 0,
            Descending = 1
        }

        public static XLSortOrder GetSortOrder(SortOrder order)
        {
            switch ((int)order)
            {
                case 1: return XLSortOrder.Descending;
                case 0:
                default:
                    return XLSortOrder.Ascending;
            }
        }
    }
}
