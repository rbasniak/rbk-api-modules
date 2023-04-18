using rbkApiModules.Commons.Core;

namespace Demo1.Models.Read;

public class PerformanceTest3
{
    public Guid Id { get; set; }


    [JsonColumn]
    public DataModel? Data { get; set; } 

    public class DataModel
    {
        [JsonColumn]
        public SimpleNamedEntity? Blog { get; set; }

        [JsonColumn]
        public SimpleNamedEntity? Author { get; set; }

        public string? Title { get; set; }

        public string? Body { get; set; }
    }
}