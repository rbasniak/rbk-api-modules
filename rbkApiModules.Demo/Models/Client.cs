using rbkApiModules.Infrastructure.Models;
using rbkApiModules.Payment.SqlServer;
using rbkApiModules.UIAnnotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace rbkApiModules.Demo.Models
{
    public class Client : BaseClient
    {
        protected Client()
        {

        }

        public Client(string name, DateTime birthdate, Plan plan)
            : base(plan)
        {
            Name = name;
            Birthdate = birthdate;
        }

        [Required, MinLength(3), MaxLength(20)]
        [DialogData(OperationType.CreateAndUpdate, "Nome", TextAreaRows = 15)]
        public string Name { get; private set; }

        [Required]
        [DialogData(OperationType.CreateAndUpdate, "Data de Nascimento")]
        public DateTime Birthdate { get; private set; }

        public ClientUser User { get; private set; }
    }
}
