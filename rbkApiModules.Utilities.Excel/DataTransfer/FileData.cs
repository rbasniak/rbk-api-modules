﻿using System.IO;

namespace rbkApiModules.Utilities.Excel;

public class FileData
{
    public MemoryStream FileStream { get; set; }
    public string ContentType { get; set; }
    public string FileName { get; set; }
}
