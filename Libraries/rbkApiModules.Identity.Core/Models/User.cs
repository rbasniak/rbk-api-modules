using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Identity.Core;

public class User : TenantEntity
{
    private readonly string _defaultAvatar = "data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMjQiIGhlaWdodD0iMjQiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyIgZmlsbC1ydWxlPSJldmVub2RkIiBjbGlwLXJ1bGU9ImV2ZW5vZGQiPjxwYXRoIGQ9Ik0yNCAyNGgtMjR2LTI0aDI0djI0em0tMi0yMmgtMjB2MjBoMjB2LTIwem0tNC4xMTggMTQuMDY0Yy0yLjI5My0uNTI5LTQuNDI3LS45OTMtMy4zOTQtMi45NDUgMy4xNDYtNS45NDIuODM0LTkuMTE5LTIuNDg4LTkuMTE5LTMuMzg4IDAtNS42NDMgMy4yOTktMi40ODggOS4xMTkgMS4wNjQgMS45NjMtMS4xNSAyLjQyNy0zLjM5NCAyLjk0NS0yLjA0OC40NzMtMi4xMjQgMS40OS0yLjExOCAzLjI2OWwuMDA0LjY2N2gxNS45OTNsLjAwMy0uNjQ2Yy4wMDctMS43OTItLjA2Mi0yLjgxNS0yLjExOC0zLjI5eiIvPjwvc3ZnPg==";
    protected HashSet<UserToClaim> _claims;
    protected HashSet<UserToRole> _roles;

    protected User()
    {

    }

    public User(string tenant, string username, string email, string password, string avatarPath, string displayName, Dictionary<string, string> metadata = null)
    {
        TenantId = tenant;
        DisplayName = displayName;
        Username = username?.ToLower();
        Email = email?.ToLower();
        Metadata = metadata;
        CreationDate = DateTime.UtcNow;
        IsActive = true;

        if (metadata == null)
        {
            Metadata = new Dictionary<string, string>();
        }

        if (!String.IsNullOrEmpty(password))
        {
            ChangePassword(password);
        }

        if (!String.IsNullOrEmpty(avatarPath))
        {
            Avatar = avatarPath;
        }
        else
        {
            Avatar = _defaultAvatar;
        }

        _claims = new HashSet<UserToClaim>();
        _roles = new HashSet<UserToRole>();

        ActivationCode = Base32GuidEncoder.EncodeId(Guid.NewGuid()) + Base32GuidEncoder.EncodeId(Guid.NewGuid()) + Base32GuidEncoder.EncodeId(Guid.NewGuid());
    }

    [Required, MinLength(3), MaxLength(255)]
    public virtual string Username { get; protected set; }

    [Required, MinLength(5), MaxLength(255)]
    public virtual string Email { get; protected set; }

    [Required, MinLength(1), MaxLength(4096)]
    public virtual string Password { get; protected set; }

    [MaxLength(128)]
    public virtual string RefreshToken { get; protected set; }

    [MaxLength(32)]
    public virtual string DisplayName { get; protected set; }

    [MaxLength(1024)]
    public virtual string Avatar { get; protected set; }

    public virtual bool IsConfirmed { get; protected set; }
    public virtual DateTime CreationDate { get; protected set; }

    [MaxLength(255)]
    public virtual string ActivationCode { get; protected set; }

    public virtual PasswordRedefineCode PasswordRedefineCode { get; protected set; }

    public virtual DateTime RefreshTokenValidity { get; protected set; }

    public virtual DateTime? LastLogin { get; protected set; }
    public virtual bool IsActive { get; protected set; }

    [JsonColumn]
    public virtual Dictionary<string, string> Metadata { get; protected set; } 

    public virtual IEnumerable<UserToRole> Roles => _roles?.ToList();

    public virtual IEnumerable<UserToClaim> Claims => _claims?.ToList();

    public void Confirm()
    {
        IsConfirmed = true;
        ActivationCode = null;
    }

    public void SetPasswordRedefineCode()
    {
        PasswordRedefineCode = new PasswordRedefineCode(DateTime.UtcNow);
    }

    public void UsePasswordRedefineCode()
    {
        //TODO: Verificar se pode ser definido como nulo ou como vazio
        PasswordRedefineCode = null;
    }

    public void UpdateDetails(string displayName, string email)
    {
        DisplayName = displayName;
        Email = email;
    }

    public void ActivateUser()
    {
        IsActive = true;
    }

    public void DeactivateUser()
    {
        IsActive = false;
    }

    /// <summary>
    /// Método que processa as roles e claims de um usuário 
    /// e retorma uma lista compilada apenas do que do usuário tem acesso
    /// </summary>
    public virtual Claim[] GetAccessClaims()
    {
        var claims = new HashSet<Claim>();

        if (_roles == null) throw new ApplicationException("User relationships need to be fully loaded from database to check the access claims");

        foreach (var role in _roles)
        {
            if (role.Role == null) throw new ApplicationException("User relationships need to be fully loaded from database to check the access claims");

            foreach (var claim in role.Role.Claims)
            {
                claims.Add(claim.Claim);
            }
        }

        if (Claims == null) throw new ApplicationException("User relationships need to be fully loaded from database to check the access claims");

        foreach (var overridedClaim in Claims)
        {
            if (overridedClaim.Access == ClaimAccessType.Allow)
            {
                claims.Add(overridedClaim.Claim);
            }
            else
            {
                claims.Remove(overridedClaim.Claim);
            }
        }

        return claims.OrderBy(x => x.Description).ToArray();
    }

    /// <summary>
    /// Método que recebe a senha do usuário e seta o valor já encriptado com hash
    /// </summary>
    /// <param name="password">Senha do usuário</param>
    public virtual void ChangePassword(string password)
    {
        Password = PasswordHasher.GenerateSaltedHash(password);
        PasswordRedefineCode = null;
    }

    /// <summary>
    /// Método para setar a validade do RefreshToken do usuário em minutos
    /// </summary>
    public virtual void SetRefreshToken(string refreshToken, double duration)
    {
        RefreshToken = refreshToken;
        RefreshTokenValidity = DateTime.UtcNow.AddMinutes(duration);
    }

    /// <summary>
    /// Método para adicionar uma role a um usuário
    /// </summary>
    public virtual UserToRole AddRole(Role role)
    {
        if (_roles == null) throw new Exception("User relationships need to be fully loaded from database to check the access claims");

        var userToRole = new UserToRole(this, role);

        _roles.Add(userToRole);

        return userToRole;
    }

    /// <summary>
    /// Método para remover uma role de um usuário
    /// </summary>
    public virtual void RemoveRole(Role role)
    {
        if (_roles == null) throw new Exception("User relationships need to be fully loaded from database to check the access claims");

        var userToRole = _roles.Single(x => x.RoleId == role.Id);

        _roles.Remove(userToRole);
    } 

    /// <summary>
    /// Adiciona uma nova claim diretamente ao usuário
    /// </summary>
    /// <param name="claim">Claim sendo adicionada</param>
    /// <param name="access">Tipo de acesso (permitir ou bloquear)</param>
    /// <returns>Retorna a entidade n-n necessária para modelar o relacionamento no EFCore</returns>
    public virtual UserToClaim AddClaim(Claim claim, ClaimAccessType access)
    {
        if (_claims == null) throw new Exception("User relationships need to be fully loaded from database to check the access claims");

        var roleToClaim = new UserToClaim(this, claim, access);

        _claims.Add(roleToClaim);

        return roleToClaim;
    }

    public virtual void SetMetadata(Dictionary<string, string> metadata)
    {
        Metadata = metadata;
    } 

    public void RefreshLastLogin()
    {
        LastLogin = DateTime.UtcNow;
        PasswordRedefineCode = null;
    }

    public override string ToString()
    {
        return $"[{TenantId}] {Username}";
    } 
} 