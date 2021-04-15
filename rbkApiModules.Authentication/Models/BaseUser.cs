using rbkApiModules.Infrastructure.Models;
using rbkApiModules.Utilities.Avatar;
using rbkApiModules.Utilities.Passwords;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace rbkApiModules.Authentication
{
    /// <summary>
    /// Classe que representa um usuário do sistema
    /// </summary>
    public abstract class BaseUser : BaseEntity
    {
        private readonly string _defaultAvatar = "data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMjQiIGhlaWdodD0iMjQiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyIgZmlsbC1ydWxlPSJldmVub2RkIiBjbGlwLXJ1bGU9ImV2ZW5vZGQiPjxwYXRoIGQ9Ik0yNCAyNGgtMjR2LTI0aDI0djI0em0tMi0yMmgtMjB2MjBoMjB2LTIwem0tNC4xMTggMTQuMDY0Yy0yLjI5My0uNTI5LTQuNDI3LS45OTMtMy4zOTQtMi45NDUgMy4xNDYtNS45NDIuODM0LTkuMTE5LTIuNDg4LTkuMTE5LTMuMzg4IDAtNS42NDMgMy4yOTktMi40ODggOS4xMTkgMS4wNjQgMS45NjMtMS4xNSAyLjQyNy0zLjM5NCAyLjk0NS0yLjA0OC40NzMtMi4xMjQgMS40OS0yLjExOCAzLjI2OWwuMDA0LjY2N2gxNS45OTNsLjAwMy0uNjQ2Yy4wMDctMS43OTItLjA2Mi0yLjgxNS0yLjExOC0zLjI5eiIvPjwvc3ZnPg==";
        protected HashSet<UserToClaim> _claims;
        protected HashSet<UserToRole> _roles;

        protected BaseUser()
        {

        }

        public BaseUser(string username, string password, string avatar, string displayName, string authenticationGroup)
        {
            if (authenticationGroup.Length > 32)
            {
                throw new Exception("Authentication group cannot have more than 32 characters.");
            }

            DisplayName = displayName;
            Username = username.ToLower();
            SetPassword(password);

            if (!String.IsNullOrEmpty(avatar))
            {
                Avatar = avatar;
            }
            else
            {
                Avatar = _defaultAvatar;
            }

            AuthenticationGroup = authenticationGroup;

            _claims = new HashSet<UserToClaim>();
            _roles = new HashSet<UserToRole>();
        }

        [Required, MinLength(3), MaxLength(255)]
        public virtual string Username { get; private set; }

        [Required, MinLength(1), MaxLength(4096)]
        public virtual string Password { get; private set; }

        [MaxLength(128)]
        public virtual string RefreshToken { get; private set; }

        [MinLength(1), MaxLength(32)]
        public virtual string AuthenticationGroup { get; private set; }

        [MaxLength(32)]
        public virtual string DisplayName { get; private set; }

        [MaxLength(1024)]
        public virtual string Avatar { get; private set; }

        public virtual bool IsConfirmed { get; private set; }

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
