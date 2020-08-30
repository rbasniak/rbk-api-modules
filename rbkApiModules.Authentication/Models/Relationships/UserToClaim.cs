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

        public UserToClaim(BaseUser user, Claim claim, ClaimAcessType access)
        {
            User = user;
            Claim = claim;
            Access = access;
        }

        public virtual Guid UserId { get; private set; }
        public virtual BaseUser User { get; private set; }

        public virtual Guid ClaimId { get; private set; }
        public virtual Claim Claim { get; private set; }

        public virtual ClaimAcessType Access { get; private set; } 
    }
}
