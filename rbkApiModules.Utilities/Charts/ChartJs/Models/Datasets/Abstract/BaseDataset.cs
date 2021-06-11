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

        public string Id { get; internal set; }
        public string Label { get; internal set; }
        public bool Normalized { get; internal set; }
        public IndexAxis? IndexAxis { get; internal set; }
        public int? Order { get; internal set; }
        public bool? Stack { get; internal set; } 
    } 
}
