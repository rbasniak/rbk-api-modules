﻿using rbkApiModules.Commons.Core;
using System;
using System.ComponentModel.DataAnnotations;

namespace GCAB.Models.Domain
{
    public class Fic : BaseEntity
    {
        public Fic(string name, FicCategory category, ChangeRequest changeRequest)
        {
            Name = name;
            Category = category;
            ChangeRequest = changeRequest;
        }

        public virtual string Name { get; set; }

        public virtual Guid CategoryId { get; set; }
        public virtual FicCategory Category { get; set; }

        public virtual Guid ChangeRequestId { get; set; }
        public virtual ChangeRequest ChangeRequest { get; set; }
    }
}
