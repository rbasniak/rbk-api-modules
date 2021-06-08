using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public abstract class BaseDataset
    {
        public BaseDataset(string id)
        {
            Id = id;
            Normalized = true;
        }

        public string Id { get; set; }
        public string Label { get; set; }
        public bool Normalized { get; set; }
        public IndexAxis? IndexAxis { get; set; }
        public int? Order { get; set; }
        public bool? Stack { get; set; } 
    } 
}
