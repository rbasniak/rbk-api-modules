using System;
using System.Collections.Generic;
using System.Text;

namespace rbkApiModules.Infrastructure.Models
{
    public class SimpleNamedEntity : BaseDataTransferObject
    {
        public SimpleNamedEntity()
        {

        }

        public SimpleNamedEntity(Guid id, string name)
        {
            Id = id.ToString();
            Name = name;
        }

        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
