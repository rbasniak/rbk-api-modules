using rbkApiModules.Infrastructure;
using rbkApiModules.Infrastructure.Models;
using System.Collections.Generic;
using System.Linq;

namespace rbkApiModules.Authentication
{
    /// <summary>
    /// Classe que representa uma permissão da aplicação (ex. criar pedido, migrar uo, etc)
    /// </summary>
    public class Claim: BaseEntity
    {
        private HashSet<UserToClaim> _users;
        private HashSet<RoleToClaim> _roles;

        protected Claim()
        {

        }

        public Claim(string name, string description)
        {
            Description = description;
            Name = name;

            _users = new HashSet<UserToClaim>();
            _roles = new HashSet<RoleToClaim>();
        }


        public virtual string Name { get; set; }
 
        public virtual string Description { get; set; }

        public virtual IEnumerable<UserToClaim> Users => _users?.ToList();

        public virtual IEnumerable<RoleToClaim> Roles => _roles?.ToList();
    }
}
