using rbkApiModules.Commons.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GCAB.Models.Domain
{
    public class ChangeRequestType: BaseEntity
    {
        private readonly HashSet<ChangeRequest> _changeRequests;

        public ChangeRequestType(string name)
        {
            _changeRequests = new HashSet<ChangeRequest>();

            Name = name;
        }

        public virtual string Name { get; set; }

        public virtual IEnumerable<ChangeRequest> ChangeRequests => _changeRequests.ToList();
    }
}
