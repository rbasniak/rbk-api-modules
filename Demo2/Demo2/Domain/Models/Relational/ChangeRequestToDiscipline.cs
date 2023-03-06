using rbkApiModules.Commons.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GCAB.Models.Domain
{
    public class ChangeRequestToDiscipline
    {

        public ChangeRequestToDiscipline(ChangeRequest changeRequest, Discipline discipline)
        {
            ChangeRequest = changeRequest;
            Discipline = discipline;
        }

        public virtual Guid ChangeRequestId { get; set; }

        public virtual ChangeRequest ChangeRequest { get; set; }

        public virtual Guid DisciplineId { get; set; }

        public virtual Discipline Discipline { get; set; }


    }
}
