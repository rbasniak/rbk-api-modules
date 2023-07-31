using System.Text;

namespace rbkApiModules.Utilities.Excel;

public static class StringSanitizer
{
    private static bool[] _lookupTable = null;

    public static string RemoveSpecialCharacters(string str)
    {
        if (_lookupTable is null)
        {
            BuildLookupTable();
        }

        StringBuilder sb = new StringBuilder();
        foreach (char c in str)
        {
            if (c < _lookupTable.Length)
            {
                if (_lookupTable[c])
                {
                    sb.Append(c);
                }
            }
        }
        return sb.ToString();
    }

    private static void BuildLookupTable()
    {
        _lookupTable = new bool[1114112];
        for (int c = 0x20; c <= 0xD7FF; c++) _lookupTable[c] = true;
        for (int c = 0xE000; c <= 0xFFFD; c++) _lookupTable[c] = true;
        for (int c = 0x10000; c <= 0x10FFFF; c++) _lookupTable[c] = true;
        _lookupTable[0x9] = true;
        _lookupTable[0xA] = true;
        _lookupTable[0xD] = true;
    }
}
