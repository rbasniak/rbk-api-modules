
namespace Demo1.Models.Read;

public class PerformanceTest1
{
    public Guid Id { get; set; }

    public Guid? BlogId { get; set; }
    public string? Blog { get; set; }

    public Guid? AuthorId { get; set; }
    public string? Author { get; set; }

    public string? Title { get; set; }
    
    public string? Body { get; set; }
}