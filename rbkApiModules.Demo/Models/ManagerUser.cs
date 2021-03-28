using rbkApiModules.Authentication;

namespace rbkApiModules.Demo.Models
{
    public class ManagerUser: BaseUser
    {
        protected ManagerUser()
        {

        } 

        public ManagerUser(string username, string password, bool isConfirmed, Manager manager) : base(username, password, "", "Fulano", "manager")
        {
            Manager = manager;
        }

        public bool IsAdmin { get; private set; }

        public Manager Manager { get; private set; }
    }
}
