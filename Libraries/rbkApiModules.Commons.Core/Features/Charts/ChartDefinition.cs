using System.Dynamic;

namespace rbkApiModules.Commons.Charts
{
    public class ChartDefinition
    {
        public ChartDefinition() 
        {
           
        }

        public string Id { get; set; }
        public ExpandoObject Chart { get; set; }
    }
}
