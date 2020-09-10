using rbkApiModules.Infrastructure.Models;
using rbkApiModules.Utilities.Passwords;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rbkApiModules.Authentication
{
    /// <summary>
    /// Classe que representa um usuário do sistema
    /// </summary>
    public abstract class BaseUser : BaseEntity
    {
        protected HashSet<UserToClaim> _claims;
        protected HashSet<UserToRole> _roles;

        protected BaseUser()
        {

        }

        public BaseUser(string username, string password)
        {
            Username = username.ToLower();
            SetPassword(password);

            _claims = new HashSet<UserToClaim>();
            _roles = new HashSet<UserToRole>();
        }

        public virtual string Username { get; private set; }

        public virtual string Password { get; private set; }

        public virtual string RefreshToken { get; private set; }

        public virtual DateTime RefreshTokenValidity { get; private set; }

        public virtual IEnumerable<UserToRole> Roles => _roles?.ToList();

        public virtual IEnumerable<UserToClaim> Claims => _claims?.ToList();

        /// <summary>
        /// Método que processa as roles e claims de um usuário 
        /// e retorma uma lista compilada apenas do que do usuário tem acesso
        /// </summary>
        public virtual List<string> GetAccessClaims()
        {
            var claims = new HashSet<string>();

            if (_roles == null) throw new ApplicationException("O usuário precisa estar carregado completamente do banco para verificar as regras de acesso.");

            foreach (var role in _roles)
            {
                if (role.Role == null) throw new ApplicationException("O usuário precisa estar carregado completamente do banco para verificar as regras de acesso.");

                foreach (var claim in role.Role.Claims)
                {
                    claims.Add(claim.Claim.Name);
                }
            }

            if (Claims == null) throw new ApplicationException("O usuário precisa estar carregado completamente do banco para verificar as regras de acesso.");

            foreach (var overridedClaim in Claims)
            {
                if (overridedClaim.Access == ClaimAcessType.Allow)
                {
                    claims.Add(overridedClaim.Claim.Name);
                }
                else
                {
                    claims.Remove(overridedClaim.Claim.Name);
                }
            }

            return claims.ToList().OrderBy(x => x).ToList();
        }

        /// <summary>
        /// Método que recebe a senha do usuário e seta o valor já encriptado com hash
        /// </summary>
        /// <param name="password">Senha do usuário</param>
        public virtual void SetPassword(string password)
        {
            Password = PasswordHasher.GenerateSaltedHash(password);
        }

        /// <summary>
        /// Método para setar o RefreshToken do usuário
        /// </summary>
        public virtual void SetRefreshToken(string refreshToken, int minutes)
        {
            RefreshToken = refreshToken;
            RefreshTokenValidity = DateTime.UtcNow.AddMinutes(minutes);
        }

        /// <summary>
        /// Método para adicionar uma role a um usuário
        /// </summary>
        public virtual UserToRole AddRole(Role role)
        {
            if (_roles == null) throw new Exception("Não é possível manipular listas que não foram carregadas completamente do banco de dados");

            var userToRole = new UserToRole(this, role);

            _roles.Add(userToRole);

            return userToRole;
        }

        /// <summary>
        /// Método para remover uma role de um usuário
        /// </summary>
        public virtual void RemoveRole(Role role)
        {
            if (_roles == null) throw new Exception("Não é possível manipular listas que não foram carregadas completamente do banco de dados");

            var userToRole = _roles.Single(x => x.RoleId == role.Id);

            _roles.Remove(userToRole); 
        }

        /// <summary>
        /// Método para adicionar uma role ao usuário
        /// </summary>
        public virtual void SetRole(Role role)
        {
            var entity = new UserToRole(this, role);

            _roles.Add(entity);
        }

        /// <summary>
        /// Adiciona uma nova claim diretamente ao usuário
        /// </summary>
        /// <param name="claim">Claim sendo adicionada</param>
        /// <param name="access">Tipo de acesso (permitir ou bloquear)</param>
        /// <returns>Retorna a entidade n-n necessária para modelar o relacionamento no EFCore</returns>
        public virtual UserToClaim AddClaim(Claim claim, ClaimAcessType access)
        {
            if (_claims == null) throw new Exception("Não é possível manipular listas que não foram carregadas completamente do banco de dados");

            var roleToClaim = new UserToClaim(this, claim, access);

            _claims.Add(roleToClaim);

            return roleToClaim;
        } 
    }
}
