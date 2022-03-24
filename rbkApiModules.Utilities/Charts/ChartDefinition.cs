using System.Dynamic;

namespace rbkApiModules.Utilities.Charts
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
