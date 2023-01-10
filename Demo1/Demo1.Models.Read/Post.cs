using rbkApiModules.Commons.Core;

namespace Demo1.Models.Read;

public class Blog: BaseEntity
{
    public string? Title { get; set; }

    public string? Description { get; set; }
}