using rbkApiModules.Infrastructure.Models;
using rbkApiModules.UIAnnotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace rbkApiModules.Demo.Models
{
    public class Client: BaseEntity
    {
        protected Client()
        {

        }

        public Client(string name, DateTime birthdate)
        {

        }

        [Required, MinLength(3), MaxLength(20)]
        [DialogData(OperationType.CreateAndUpdate, "Nome")]
        public string Name { get; private set; }

        [Required]
        [DialogData(OperationType.CreateAndUpdate, "Data de Nascimento")]
        public DateTime Birthdate { get; private set; }

        public User User { get; private set; }
    }
}
