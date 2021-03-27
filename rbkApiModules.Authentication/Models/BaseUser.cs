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
        private readonly string _defaultAvatar = "data:image/svg+xml;base64,PHN2ZyB2aWV3Qm94PSItNDIgMCA1MTIgNTEyIiB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciPjxwYXRoIGQ9Im0zMzMuNjcxODc1IDEyMy4zMDg1OTRjMCAzMy44ODY3MTgtMTIuMTUyMzQ0IDYzLjIxODc1LTM2LjEyNSA4Ny4xOTUzMTItMjMuOTcyNjU2IDIzLjk3MjY1Ni01My4zMDg1OTQgMzYuMTI1LTg3LjE5NTMxMyAzNi4xMjVoLS4wNTg1OTNjLTMzLjg0Mzc1LS4wMTE3MTgtNjMuMTYwMTU3LTEyLjE2NDA2Mi04Ny4xMzI4MTMtMzYuMTI1LTIzLjk3NjU2Mi0yMy45NzY1NjItMzYuMTI1LTUzLjMwODU5NC0zNi4xMjUtODcuMTk1MzEyIDAtMzMuODc1IDEyLjE0ODQzOC02My4yMTA5MzggMzYuMTI1LTg3LjE4MzU5NCAyMy45NjA5MzgtMjMuOTY0ODQ0IDUzLjI3NzM0NC0zNi4xMTMyODEyIDg3LjEzMjgxMy0zNi4xMjVoLjA1ODU5M2MzMy44NzUgMCA2My4yMTA5MzggMTIuMTUyMzQ0IDg3LjE5NTMxMyAzNi4xMjUgMjMuOTcyNjU2IDIzLjk3MjY1NiAzNi4xMjUgNTMuMzA4NTk0IDM2LjEyNSA4Ny4xODM1OTR6bTAgMCIgZmlsbD0iI2ZmYmI4NSIvPjxwYXRoIGQ9Im00MjcuMTY3OTY5IDQyMy45NDUzMTJjMCAyNi43MzQzNzYtOC41MDM5MDcgNDguMzc4OTA3LTI1LjI1MzkwNyA2NC4zMjAzMTMtMTYuNTU0Njg3IDE1Ljc1MzkwNi0zOC40NDkyMTggMjMuNzM0Mzc1LTY1LjA3MDMxMiAyMy43MzQzNzVoLTI0Ni41MzEyNWMtMjYuNjIxMDk0IDAtNDguNTE1NjI1LTcuOTgwNDY5LTY1LjA1ODU5NC0yMy43MzQzNzUtMTYuNzYxNzE4LTE1Ljk1MzEyNS0yNS4yNTM5MDYtMzcuNTkzNzUtMjUuMjUzOTA2LTY0LjMyMDMxMyAwLTEwLjI4MTI1LjMzOTg0NC0yMC40NTMxMjQgMS4wMTk1MzEtMzAuMjM0Mzc0LjY5MTQwNy0xMCAyLjA4OTg0NC0yMC44ODI4MTMgNC4xNTIzNDQtMzIuMzYzMjgyIDIuMDc4MTI1LTExLjU3NDIxOCA0Ljc1LTIyLjUxNTYyNSA3Ljk0OTIxOS0zMi41MTU2MjUgMy4zMjAzMTItMTAuMzUxNTYyIDcuODEyNS0yMC41NjI1IDEzLjM3MTA5NC0zMC4zNDM3NSA1Ljc3MzQzNy0xMC4xNTIzNDMgMTIuNTU0Njg3LTE4Ljk5NjA5MyAyMC4xNTYyNS0yNi4yNzczNDMgNy45Njg3NS03LjYyMTA5NCAxNy43MTA5MzctMTMuNzQyMTg4IDI4Ljk3MjY1Ni0xOC4yMDMxMjYgMTEuMjIyNjU2LTQuNDM3NSAyMy42NjQwNjItNi42ODc1IDM2Ljk3NjU2Mi02LjY4NzUgNS4yMjI2NTYgMCAxMC4yODEyNSAyLjEzNjcxOSAyMC4wMzEyNSA4LjQ4ODI4MiA2LjEwMTU2MyAzLjk4MDQ2OCAxMy4xMzI4MTMgOC41MTE3MTggMjAuODk0NTMyIDEzLjQ3MjY1NiA2LjcwMzEyNCA0LjI3MzQzOCAxNS43ODEyNSA4LjI4MTI1IDI3LjAwMzkwNiAxMS45MDIzNDQgOS44NjMyODEgMy4xOTE0MDYgMTkuODc1IDQuOTcyNjU2IDI5Ljc2NTYyNSA1LjI4MTI1IDEuMDg5ODQzLjAzOTA2MiAyLjE3OTY4Ny4wNTg1OTQgMy4yNjk1MzEuMDU4NTk0IDEwLjk4NDM3NSAwIDIyLjA5Mzc1LTEuODAwNzgyIDMzLjA0Njg3NS01LjMzOTg0NCAxMS4yMjI2NTYtMy42MjEwOTQgMjAuMzEyNS03LjYyODkwNiAyNy4wMTE3MTktMTEuOTAyMzQ0IDcuODQzNzUtNS4wMTE3MTkgMTQuODc1LTkuNTM5MDYyIDIwLjg4NjcxOC0xMy40NjA5MzggOS43NTc4MTMtNi4zNjMyODEgMTQuODA4NTk0LTguNSAyMC4wNDI5NjktOC41IDEzLjMwMDc4MSAwIDI1Ljc0MjE4OCAyLjI1IDM2Ljk3MjY1NyA2LjY4NzUgMTEuMjYxNzE4IDQuNDYwOTM4IDIxLjAwMzkwNiAxMC41OTM3NSAyOC45NjQ4NDMgMTguMjAzMTI2IDcuNjEzMjgxIDcuMjgxMjUgMTQuMzk0NTMxIDE2LjEyNSAyMC4xNjQwNjMgMjYuMjc3MzQzIDUuNTYyNSA5Ljc4OTA2MyAxMC4wNjI1IDE5Ljk5MjE4OCAxMy4zNzEwOTQgMzAuMzMyMDMxIDMuMjAzMTI0IDEwLjAxMTcxOSA1Ljg4MjgxMiAyMC45NTMxMjYgNy45NjA5MzcgMzIuNTM1MTU3IDIuMDUwNzgxIDExLjQ5MjE4NyAzLjQ1MzEyNSAyMi4zNzUgNC4xNDA2MjUgMzIuMzQ3NjU2LjY5MTQwNiA5Ljc1IDEuMDMxMjUgMTkuOTIxODc1IDEuMDQyOTY5IDMwLjI0MjE4N3ptMCAwIiBmaWxsPSIjNmFhOWZmIi8+PHBhdGggZD0ibTIxMC4zNTE1NjIgMjQ2LjYyODkwNmgtLjA1ODU5M3YtMjQ2LjYyODkwNmguMDU4NTkzYzMzLjg3NSAwIDYzLjIxMDkzOCAxMi4xNTIzNDQgODcuMTk1MzEzIDM2LjEyNSAyMy45NzI2NTYgMjMuOTcyNjU2IDM2LjEyNSA1My4zMDg1OTQgMzYuMTI1IDg3LjE4MzU5NCAwIDMzLjg4NjcxOC0xMi4xNTIzNDQgNjMuMjE4NzUtMzYuMTI1IDg3LjE5NTMxMi0yMy45NzI2NTYgMjMuOTcyNjU2LTUzLjMwODU5NCAzNi4xMjUtODcuMTk1MzEzIDM2LjEyNXptMCAwIiBmaWxsPSIjZjVhODZjIi8+PHBhdGggZD0ibTQyNy4xNjc5NjkgNDIzLjk0NTMxMmMwIDI2LjczNDM3Ni04LjUwMzkwNyA0OC4zNzg5MDctMjUuMjUzOTA3IDY0LjMyMDMxMy0xNi41NTQ2ODcgMTUuNzUzOTA2LTM4LjQ0OTIxOCAyMy43MzQzNzUtNjUuMDcwMzEyIDIzLjczNDM3NWgtMTI2LjU1MDc4MXYtMjI1LjUzNTE1NmMxLjA4OTg0My4wMzkwNjIgMi4xNzk2ODcuMDU4NTk0IDMuMjY5NTMxLjA1ODU5NCAxMC45ODQzNzUgMCAyMi4wOTM3NS0xLjgwMDc4MiAzMy4wNDY4NzUtNS4zMzk4NDQgMTEuMjIyNjU2LTMuNjIxMDk0IDIwLjMxMjUtNy42Mjg5MDYgMjcuMDExNzE5LTExLjkwMjM0NCA3Ljg0Mzc1LTUuMDExNzE5IDE0Ljg3NS05LjUzOTA2MiAyMC44ODY3MTgtMTMuNDYwOTM4IDkuNzU3ODEzLTYuMzYzMjgxIDE0LjgwODU5NC04LjUgMjAuMDQyOTY5LTguNSAxMy4zMDA3ODEgMCAyNS43NDIxODggMi4yNSAzNi45NzI2NTcgNi42ODc1IDExLjI2MTcxOCA0LjQ2MDkzOCAyMS4wMDM5MDYgMTAuNTkzNzUgMjguOTY0ODQzIDE4LjIwMzEyNiA3LjYxMzI4MSA3LjI4MTI1IDE0LjM5NDUzMSAxNi4xMjUgMjAuMTY0MDYzIDI2LjI3NzM0MyA1LjU2MjUgOS43ODkwNjMgMTAuMDYyNSAxOS45OTIxODggMTMuMzcxMDk0IDMwLjMzMjAzMSAzLjIwMzEyNCAxMC4wMTE3MTkgNS44ODI4MTIgMjAuOTUzMTI2IDcuOTYwOTM3IDMyLjUzNTE1NyAyLjA1MDc4MSAxMS40OTIxODcgMy40NTMxMjUgMjIuMzc1IDQuMTQwNjI1IDMyLjM0NzY1Ni42OTE0MDYgOS43NSAxLjAzMTI1IDE5LjkyMTg3NSAxLjA0Mjk2OSAzMC4yNDIxODd6bTAgMCIgZmlsbD0iIzI2ODJmZiIvPjwvc3ZnPg==";
        protected HashSet<UserToClaim> _claims;
        protected HashSet<UserToRole> _roles;

        protected BaseUser()
        {

        }

        public BaseUser(string username, string password, string avatar, string authenticationGroup)
        {
            if (authenticationGroup.Length > 32)
            {
                throw new Exception("Authentication group cannot have more than 32 characters.");
            }

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

        [Required, MinLength(1), MaxLength(32)]
        public virtual string AuthenticationGroup { get; private set; }

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
