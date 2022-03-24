using System.Collections.Generic;

namespace rbkApiModules.Logs.Core
{
    public class FilterOptionListData
    {
        public FilterOptionListData()
        {
            Messages = new List<string>();
            Levels = new List<LogLevel>();
            Layers = new List<string>();
            Areas = new List<string>();
            Versions = new List<string>();
            Sources = new List<string>();
            Enviroments = new List<string>();
            EnviromentsVersions = new List<string>();
            Users = new List<string>();
            Domains = new List<string>();
            Machines = new List<string>();
        }

        public List<string> Messages { get; set; }
        public List<LogLevel> Levels { get; set; }
        public List<string> Layers { get; set; }
        public List<string> Areas { get; set; }
        public List<string> Versions { get; set; }
        public List<string> Sources { get; set; }
        public List<string> Enviroments { get; set; }
        public List<string> EnviromentsVersions { get; set; }
        public List<string> Users { get; set; }
        public List<string> Domains { get; set; }
        public List<string> Machines { get; set; }
    }
}