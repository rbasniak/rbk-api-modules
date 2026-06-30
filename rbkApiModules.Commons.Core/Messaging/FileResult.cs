namespace rbkApiModules.Commons.Core;

public class FileResult
{
    public required MemoryStream FileStream { get; init; }
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
}
