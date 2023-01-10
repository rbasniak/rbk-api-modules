namespace rbkApiModules.Commons.Core;

public class FileResult
{
    public MemoryStream FileStream { get; set; }
    public string ContentType { get; set; }
    public string FileName { get; set; }
}
