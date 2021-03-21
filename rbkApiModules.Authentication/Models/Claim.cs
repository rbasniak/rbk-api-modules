﻿using rbkApiModules.Infrastructure.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public Claim(string name, string description, string authenticationGroup)
        {
            AuthenticationGroup = authenticationGroup;
            Description = description;
            Name = name;

            _users = new HashSet<UserToClaim>();
            _roles = new HashSet<RoleToClaim>();
        }

        [Required, MinLength(1), MaxLength(128)]
        public virtual string Name { get; private set; }

        [Required, MinLength(1), MaxLength(128)]
        public virtual string Description { get; private set; }

        [Required, MinLength(1), MaxLength(32)]
        public virtual string AuthenticationGroup { get; private set; }

        public virtual IEnumerable<UserToClaim> Users => _users?.ToList();

        public virtual IEnumerable<RoleToClaim> Roles => _roles?.ToList();
    }
}
