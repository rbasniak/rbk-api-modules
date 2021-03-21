using rbkApiModules.Infrastructure.Models;
using System;

namespace rbkApiModules.Authentication
{
    /// <summary>
    /// Classe necessária para modelar o relacionamento n-n do EntityFrameworkCore
    /// </summary>
    public class UserToRole
    {
        protected UserToRole()
        {

        }

        public UserToRole(BaseUser user, Role role)
        {
            if (user.AuthenticationGroup != role.AuthenticationGroup)
            {
                throw new SafeException("Invalid authentication groups");
            }

            User = user;
            Role = role; 
        }

        public virtual Guid UserId { get; private set; }
        public virtual BaseUser User { get; private set; }

        public virtual Guid RoleId { get; private set; }
        public virtual Role Role { get; private set; } 
    }
}
