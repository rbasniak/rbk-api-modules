using rbkApiModules.Commons.Core;
using System.Collections.Generic;
using System.Linq;

namespace GCAB.Models.Domain
{
    public class ChangeRequestPriority: BaseEntity
    {
        private readonly HashSet<ChangeRequest> _changeRequests;

        public ChangeRequestPriority(string name, string color)
        {
            Name = name;
            Color = color;

            _changeRequests = new HashSet<ChangeRequest>();
        }

        public virtual string Name { get; set; }

        public virtual int Order { get; set; }

        public virtual string Color { get; set; }

        public virtual IEnumerable<ChangeRequest> ChangeRequests => _changeRequests.ToList(); 
    }
}
