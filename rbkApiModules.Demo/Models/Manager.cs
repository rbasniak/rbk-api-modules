using rbkApiModules.Infrastructure.Models;
using rbkApiModules.UIAnnotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace rbkApiModules.Demo.Models
{
    public class Manager : BaseEntity
    {
        protected Manager()
        {

        }

        public Manager(string name)
        {
            Name = name;
        }

        [Required, MinLength(3), MaxLength(20)]
        [DialogData(OperationType.CreateAndUpdate, "Nome")]
        public string Name { get; private set; }

        public ManagerUser User { get; private set; }
    }
}
