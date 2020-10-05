using rbkApiModules.Authentication;

namespace rbkApiModules.Demo.Models
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

        public string Test { get; set; }

        public Client Client { get; private set; }
    }
}
