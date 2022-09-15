using rbkApiModules.Infrastructure.Models;
using rbkApiModules.UIAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace rbkApiModules.Authentication
{
    /// <summary>
    /// Classe que representa uma role, que é um grupo de permissões 
    /// que pode ser associada a vários usuários
    /// </summary>
    public class Role: BaseEntity
    {
        private HashSet<RoleToClaim> _claims;
        private HashSet<UserToRole> _users;

        protected Role()
        {

        }

        public Role(string name, string authenticationGroup)
        {
            Name = name;
            AuthenticationGroup = authenticationGroup;

            _users = new HashSet<UserToRole>();
            _claims = new HashSet<RoleToClaim>();

            this.Validate();
        }

        [Required, MinLength(1), MaxLength(128)]
        [DialogData(OperationType.CreateAndUpdate, "Nome")]
        public virtual string Name { get; private set; }

        [Required, MinLength(1), MaxLength(32)]
        public virtual string AuthenticationGroup { get; private set; }

        public virtual IEnumerable<RoleToClaim> Claims => _claims?.ToList();

        public virtual IEnumerable<UserToRole> Users => _users?.ToList();

        public RoleToClaim AddClaim(Claim claim)
        {
            if (_claims == null) throw new Exception("Não é possível manipular listas que não foram carregadas completamente do banco de dados");

            var roleToClaim = new RoleToClaim(this, claim);

            _claims.Add(roleToClaim);

            return roleToClaim;
        }

        public void SetName(string name)
        {
            Name = name;
        }
    }
}
