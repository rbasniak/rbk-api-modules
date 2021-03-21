using rbkApiModules.Authentication;

namespace rbkApiModules.Demo.Models
{
    public class ClientUser: BaseUser
    {
        protected ClientUser()
        {

        }

        public ClientUser(string username, string password, bool isConfirmed, Client client): base(username, password, "client")
        {
            IsConfirmed = isConfirmed;
            Client = client;
        }

        public bool IsConfirmed { get; set; }

        public string IsBlocked { get; set; }

        public Client Client { get; private set; }
    }
}
