using rbkApiModules.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rbkApiModules.Tester.Models
{
    public class User: BaseUser
    {
        protected User()
        {

        }

        public User(string username, string password, bool isConfirmed, Client client): base(username, password)
        {
            IsConfirmed = isConfirmed;
            Client = client;
        }

        public bool IsConfirmed { get; set; }

        public Client Client { get; private set; }
    }
}
