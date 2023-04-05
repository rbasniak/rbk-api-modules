using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Commons.Localization;

public class FilterOptionListData
{
    public FilterOptionListData()
    {
        Versions = new List<string>();
        Areas = new List<string>();
        Domains = new List<string>();
        Agents = new List<string>();
        Actions = new List<string>();
        Responses = new List<string>();
        Users = new List<string>();
    }

    public List<string> Versions { get; set; }
    public List<string> Areas { get; set; }
    public List<string> Domains { get; set; }
    public List<string> Actions { get; set; }
    public List<string> Responses { get; set; }
    public List<string> Users { get; set; }
    public List<string> Agents { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime StartDate { get; set; }
}