using rbkApiModules.Authentication;

namespace rbkApiModules.Demo.Models
{
    public class ManagerUser: BaseUser
    {
        protected ManagerUser()
        {

        } 

        public ManagerUser(string username, string password, bool isConfirmed, Manager manager) : base(username, password, "", "manager")
        {
            IsConfirmed = isConfirmed;
            Manager = manager;
        }

        public bool IsConfirmed { get; set; }

        public bool IsAdmin { get; private set; }

        public Manager Manager { get; private set; }
    }
}
