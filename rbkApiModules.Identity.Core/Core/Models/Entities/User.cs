using System.ComponentModel.DataAnnotations;
using rbkApiModules.Commons.Core.Database;

namespace rbkApiModules.Identity.Core;

public sealed class User : TenantEntity
{
    private readonly string _defaultAvatar = "data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMjQiIGhlaWdodD0iMjQiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyIgZmlsbC1ydWxlPSJldmVub2RkIiBjbGlwLXJ1bGU9ImV2ZW5vZGQiPjxwYXRoIGQ9Ik0yNCAyNGgtMjR2LTI0aDI0djI0em0tMi0yMmgtMjB2MjBoMjB2LTIwem0tNC4xMTggMTQuMDY0Yy0yLjI5My0uNTI5LTQuNDI3LS45OTMtMy4zOTQtMi45NDUgMy4xNDYtNS45NDIuODM0LTkuMTE5LTIuNDg4LTkuMTE5LTMuMzg4IDAtNS42NDMgMy4yOTktMi40ODggOS4xMTkgMS4wNjQgMS45NjMtMS4xNSAyLjQyNy0zLjM5NCAyLjk0NS0yLjA0OC40NzMtMi4xMjQgMS40OS0yLjExOCAzLjI2OWwuMDA0LjY2N2gxNS45OTNsLjAwMy0uNjQ2Yy4wMDctMS43OTItLjA2Mi0yLjgxNS0yLjExOC0zLjI5eiIvPjwvc3ZnPg==";
    private HashSet<UserToClaim> _claims;
    private HashSet<UserToRole> _roles;

    private User()
    {
        _claims = default!;
        _roles = default!;
    }

    public User(string tenant, string username, string email, string password, string avatar, 
        string displayName, AuthenticationMode authenticationMode, Dictionary<string, string>? metadata = null)
    {
        if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));
        if (string.IsNullOrEmpty(displayName)) throw new ArgumentNullException(nameof(displayName));

        AuthenticationMode = authenticationMode;
        TenantId = tenant;
        DisplayName = displayName;
        Username = username.ToLower();
        Email = email.ToLower();
        Metadata = metadata ?? new Dictionary<string, string>();
        CreationDate = DateTime.UtcNow;
        IsActive = true;

        if (!String.IsNullOrEmpty(password))
        {
            ChangePassword(password);
        }

        if (!String.IsNullOrEmpty(avatar))
        {
            Avatar = avatar;
        }
        else
        {
            Avatar = _defaultAvatar;
        }

        _claims = new HashSet<UserToClaim>();
        _roles = new HashSet<UserToRole>();

        ActivationCode = $"{Guid.NewGuid():N}{Guid.NewGuid():N}{Guid.NewGuid():N}{Guid.NewGuid():N}";
    }

    [Required, MinLength(3), MaxLength(255)]
    public string Username { get; private set; } = string.Empty;

    [Required, MinLength(5), MaxLength(255)]
    public string? Email { get; private set; } = string.Empty;

    [MinLength(1), MaxLength(4096)]
    public string? Password { get; private set; } = string.Empty;

    [MaxLength(128)]
    public string? RefreshToken { get; private set; } = string.Empty;

    public AuthenticationMode AuthenticationMode { get; private set; }

    [MaxLength(255)]
    public string DisplayName { get; private set; } = string.Empty;

    [MaxLength(1024)]
    public string Avatar { get; private set; } = string.Empty;

    public bool IsConfirmed { get; private set; }
    public DateTime CreationDate { get; private set; }

    [MaxLength(255)]
    public string? ActivationCode { get; private set; }

    public PasswordRedefineCode? PasswordRedefineCode { get; private set; }

    public DateTime? RefreshTokenValidity { get; private set; }

    public DateTime? LastLogin { get; private set; }
    public bool IsActive { get; private set; }

    [JsonColumn]
    public Dictionary<string, string> Metadata { get; private set; } = new();

    public IEnumerable<UserToRole> Roles => _roles?.AsReadOnly()!;

    public IEnumerable<UserToClaim> Claims => _claims?.AsReadOnly()!;

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
        PasswordRedefineCode = null;
    }

    public void UpdateDetails(string displayName, string email, string avatar)
    {
        DisplayName = displayName;
        Email = email;
        Avatar = avatar;
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
    public Claim[] GetAccessClaims()
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
    public void ChangePassword(string password)
    {
        Password = PasswordHasher.GenerateSaltedHash(password);
        PasswordRedefineCode = null;
    }

    /// <summary>
    /// Método para setar a validade do RefreshToken do usuário em minutos
    /// </summary>
    public void SetRefreshToken(string refreshToken, double duration)
    {
        RefreshToken = refreshToken;
        RefreshTokenValidity = DateTime.UtcNow.AddMinutes(duration);
    }

    /// <summary>
    /// Método para adicionar uma role a um usuário
    /// </summary>
    public UserToRole AddRole(Role role)
    {
        if (_roles == null) throw new Exception("User relationships need to be fully loaded from database to check the access claims");

        var userToRole = new UserToRole(this, role);

        _roles.Add(userToRole);

        return userToRole;
    }

    /// <summary>
    /// Método para remover uma role de um usuário
    /// </summary>
    public void RemoveRole(Role role)
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
    public UserToClaim AddClaim(Claim claim, ClaimAccessType access)
    {
        if (_claims == null) throw new Exception("User relationships need to be fully loaded from database to check the access claims");

        var roleToClaim = new UserToClaim(this, claim, access);

        _claims.Add(roleToClaim);

        return roleToClaim;
    }

    public void SetMetadata(Dictionary<string, string> metadata)
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