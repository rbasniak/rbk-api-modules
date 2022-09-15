using rbkApiModules.Infrastructure.Models;
using System;

namespace rbkApiModules.Authentication
{
    /// <summary>
    /// Classe necessária para modelar o relacionamento n-n do EntityFrameworkCore
    /// </summary>
    public class UserToClaim
    {
        protected UserToClaim()
        {

        }

        public UserToClaim(BaseUser user, Claim claim, ClaimAccessType access)
        {
            if (user.AuthenticationGroup != claim.AuthenticationGroup)
            {
                throw new SafeException("Invalid authentication groups");
            }

            User = user;
            Claim = claim;
            Access = access;
        }

        public virtual Guid UserId { get; private set; }
        public virtual BaseUser User { get; private set; }

        public virtual Guid ClaimId { get; private set; }
        public virtual Claim Claim { get; private set; }

        public virtual ClaimAccessType Access { get; private set; } 
    }
}
