using System.Collections.Generic;

namespace rbkApiModules.Diagnostics.Core
{
    public class FilterOptionListData
    {
        public FilterOptionListData()
        {
            Versions = new List<string>();
            Areas = new List<string>();
            Layers = new List<string>();
            Domains = new List<string>();
            Users = new List<string>();
            Browsers = new List<string>();
            Agents = new List<string>();
            Devices = new List<string>();
            OperatinSystems = new List<string>();
            Messages = new List<string>();
            Sources = new List<string>(); 
        }

        public List<string> Versions { get; set; }
        public List<string> Areas { get; set; }
        public List<string> Layers { get; set; }
        public List<string> Domains { get; set; }
        public List<string> Users { get; set; }
        public List<string> Browsers { get; set; }
        public List<string> Agents { get; set; }
        public List<string> Devices { get; set; }
        public List<string> OperatinSystems { get; set; }
        public List<string> Messages { get; set; }
        public List<string> Sources { get; set; }
    }
}