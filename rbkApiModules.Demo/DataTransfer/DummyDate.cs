using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rbkApiModules.Demo.DataTransfer
{
    public class DummyDate
    {
        public int MyProperty { get; set; }
        public SubDummyDate Child { get; set; }
    }

    public class SubDummyDate
    {
        public DateTime Date { get; set; }
        public SimpleNamedEntity<int> Test { get; set; }
    }
}
