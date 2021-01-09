using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Workflow
{
    public abstract class BaseStateEntity: BaseEntity
    {
        public BaseStateEntity()
        {

        }

        public virtual Guid StateId { get; private set; }
        public virtual State State { get; private set; }
    }
}
