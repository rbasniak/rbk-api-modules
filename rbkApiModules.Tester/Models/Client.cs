using rbkApiModules.Infrastructure.Models;
using System;

namespace rbkApiModules.Tester.Models
{
    public class Client: BaseEntity
    {
        protected Client()
        {

        }

        public Client(string name, DateTime birthdate)
        {

        }

        public string Name { get; private set; }

        public DateTime Birthdate { get; private set; }

        public User User { get; private set; }
    }
}
