﻿using rbkApiModules.Authentication;

namespace rbkApiModules.Demo.Models
{
    public class ClientUser: BaseUser
    {
        protected ClientUser()
        {

        }

        public ClientUser(string username, string email, string password, bool isConfirmed, Client client): base(username, email, password, "", client.Name, "client")
        {
            IsConfirmed = isConfirmed;
            Client = client;
        }

        public bool IsConfirmed { get; private set; }

        public string IsBlocked { get; private set; }

        public Client Client { get; private set; }
    }
}
