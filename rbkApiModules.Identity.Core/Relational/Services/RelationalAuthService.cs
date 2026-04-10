using rbkApiModules.Identity.Core;
using Microsoft.Extensions.Logging;
using rbkApiModules.Commons.Relational;

namespace rbkApiModules.Identity.Relational;

public class RelationalAuthService: IAuthService
{
    private readonly DbContext _context;
    private readonly ILocalizationService _localization;
    private readonly ILogger<RelationalAuthService> _logger;

    public RelationalAuthService(IEnumerable<DbContext> contexts, ILocalizationService localization, ILogger<RelationalAuthService> logger)
    {
        _context = contexts.GetDefaultContext();
        _localization = localization;
        _logger = logger;
    }

    public async Task ConfirmUserAsync(string username, string tenant, CancellationToken cancellationToken)
    {
        tenant = tenant != null ? tenant.ToUpper() : null;

        var user = await LoadUserEntityAsync(username, tenant, cancellationToken);

        user.Confirm();

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<User> FindUserAsync(string username, string tenant, CancellationToken cancellationToken)
    {
        tenant = tenant != null ? tenant.ToUpper() : null;

        var user = await LoadUserEntityAsync(username, tenant, cancellationToken);
        return user;
    }

    public async Task<User> GetUserWithDependenciesAsync(string username, string tenant, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting user and its dependencies from database");

        tenant = tenant != null ? tenant.ToUpper() : null;

        var user = await _context.Set<User>()
            .Include(x => x.Roles)
                .ThenInclude(x => x.Role)
                    .ThenInclude(x => x.Claims)
                        .ThenInclude(x => x.Claim)
            .Include(x => x.Claims)
                .ThenInclude(x => x.Claim)
            .SingleAsync(x => (x.Username.ToLower() == username.ToLower() || x.Email.ToLower() == username) && x.TenantId == tenant, cancellationToken);

        return user;
    }

    public async Task UpdateRefreshTokenAsync(string username, string tenant, string refreshToken, double duration, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating refresh token");

        tenant = tenant != null ? tenant.ToUpper() : null;

        var user = await LoadUserEntityAsync(username, tenant, cancellationToken);

        user.SetRefreshToken(refreshToken, duration);

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task<User> LoadUserEntityAsync(string username, string tenant, CancellationToken cancellationToken)
    {
        tenant = tenant != null ? tenant.ToUpper() : null;

        return  await _context.Set<User>()
            .SingleOrDefaultAsync(x => (x.Username.ToLower() == username.ToLower() || x.Email.ToLower() == username.ToLower()) && x.TenantId == tenant, cancellationToken);
    }

    public async Task<User> RedefinePasswordAsync(string resetPasswordCode, string password, CancellationToken cancellationToken)
    {
        var user = await _context.Set<User>()
            .Include(x => x.PasswordRedefineCode)
            .FirstOrDefaultAsync(x => x.PasswordRedefineCode.Hash == resetPasswordCode, cancellationToken);

        if (user == null) throw new ExpectedInternalException(_localization.LocalizeString(AuthenticationMessages.Erros.CouldNotFindTheUserAssociatedWithThePasswordResetCode));

        user.ChangePassword(password);

        await _context.SaveChangesAsync(cancellationToken);

        return user;
    }

    public async Task<bool> IsPasswordResetCodeValidAsync(string code, CancellationToken cancelation = default)
    {
        var user = await _context.Set<User>()
            .Include(x => x.PasswordRedefineCode)
            .SingleOrDefaultAsync(x => x.PasswordRedefineCode.Hash == code, cancellationToken: cancelation);

        return user != null
            && user.PasswordRedefineCode.CreationDate.HasValue
            && (DateTime.UtcNow - user.PasswordRedefineCode.CreationDate).Value.TotalHours <= 24;
    }

    public async Task<bool> IsRefreshTokenValidAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var user = await _context.Set<User>()
            .SingleAsync(x => x.RefreshToken == refreshToken, cancellationToken);

        return user.RefreshTokenValidity > DateTime.UtcNow;
    }

    public async Task<bool> RefreshTokenExistsOnDatabaseAsync(string refreshToken, CancellationToken cancellationToken)
    {
        return await _context.Set<User>().AnyAsync(x => x.RefreshToken == refreshToken, cancellationToken);
    }

    public async Task<User> GetUserFromRefreshtokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var user = await _context.Set<User>()
                .Include(x => x.Roles)
                    .ThenInclude(x => x.Role)
                        .ThenInclude(x => x.Claims)
                            .ThenInclude(x => x.Claim)
                .Include(x => x.Claims)
                    .ThenInclude(x => x.Claim)
                .SingleOrDefaultAsync(x => x.RefreshToken == refreshToken && x.RefreshTokenValidity > DateTime.UtcNow, cancellationToken);

        return user;
    }

    public async Task<bool> IsUserRegisteredAsync(string email, string tenant, CancellationToken cancellationToken)
    {
        tenant = tenant != null ? tenant.ToUpper() : null;

        return await _context.Set<User>().AnyAsync(x => x.Email == email && x.TenantId == tenant, cancellationToken);
    }

    public async Task<bool> IsUserConfirmedAsync(string email, string tenant, CancellationToken cancellationToken)
    {
        tenant = tenant != null ? tenant.ToUpper() : null;

        return await _context.Set<User>().AnyAsync(x => x.Email.ToLower() == email.ToLower() && x.IsConfirmed && x.TenantId == tenant, cancellationToken);
    }

    public async Task RequestPasswordResetAsync(string email, string tenant, CancellationToken cancellationToken)
    {
        tenant = tenant != null ? tenant.ToUpper() : null;

        var user = await LoadUserEntityAsync(email, tenant, cancellationToken);

        user.SetPasswordRedefineCode();

        await _context.SaveChangesAsync();
    }

    public async Task RefreshLastLogin(string username, string tenant, CancellationToken cancellationToken)
    {
        tenant = tenant != null ? tenant.ToUpper() : null;

        var user = await LoadUserEntityAsync(username, tenant, cancellationToken);

        user.RefreshLastLogin();

        await _context.SaveChangesAsync();
    }

    public async Task<User[]> GetAllAsync(string tenant, CancellationToken cancellationToken)
    {
        tenant = tenant != null ? tenant.ToUpper() : null;

        var claims = await _context.Set<Claim>().ToListAsync();
        var roles = await _context.Set<Role>().Include(x => x.Claims).ThenInclude(x => x.Claim).ToListAsync();

        var users = await _context.Set<User>()
            .Include(x => x.Roles)
                .ThenInclude(x => x.Role)
            .Include(x => x.Claims)
            .OrderBy(x => x.Username)
            .Where(x => x.TenantId == tenant)
            .ToArrayAsync(cancellationToken);

        return users;
    }

    public async Task<User[]> GetAllWithRoleAsync(string userTenant, string roleTenant, string roleName, CancellationToken cancellationToken)
    {
        User[] users;

        if (String.IsNullOrEmpty(roleTenant))
        {
            users = await _context.Set<User>()
                .Include(x => x.Roles)
                    .ThenInclude(x => x.Role)
                .OrderBy(x => x.Username)
                .Where(x => x.TenantId != null && x.TenantId.ToLower() == userTenant.ToLower() && x.Roles.Any(x => x.Role.Name.ToLower() == roleName.ToLower() && x.Role.TenantId == null))
                .ToArrayAsync(cancellationToken);
        }
        else
        {
            users = await _context.Set<User>()
                .Include(x => x.Roles)
                    .ThenInclude(x => x.Role)
                .OrderBy(x => x.Username)
                .Where(x => x.TenantId != null && x.TenantId.ToLower() == userTenant.ToLower() && x.Roles.Any(x => x.Role.Name.ToLower() == roleName.ToLower() && x.Role.TenantId != null && x.Role.TenantId.ToLower() == roleTenant.ToLower()))
                .ToArrayAsync(cancellationToken);
        }

        return users;
    }

    public async Task<User> ReplaceRoles(string username, string tenant, Guid[] roleIds, CancellationToken cancellationToken)
    {
        tenant = tenant != null ? tenant.ToUpper() : null;

        var user = await GetUserWithDependenciesAsync(username, tenant, cancellationToken);

        _context.RemoveRange(user.Roles);

        await _context.SaveChangesAsync();

        foreach (var roleId in roleIds)
        {
            var role = await _context.Set<Role>().Include(x => x.Claims).ThenInclude(x => x.Claim).SingleAsync(x => x.Id == roleId);
            user.AddRole(role);
        }

        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User> AddRole(string username, string tenant, Guid roleId, CancellationToken cancellationToken)
    {
        tenant = tenant != null ? tenant.ToUpper() : null;

        var user = await LoadUserEntityAsync(username, tenant, cancellationToken);

        var role = await _context.Set<Role>().SingleAsync(x => x.Id == roleId);
        user.AddRole(role);

        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User> RemoveRole(string username, string tenant, Guid roleId, CancellationToken cancellationToken)
    {
        tenant = tenant != null ? tenant.ToUpper() : null;

        var user = await LoadUserEntityAsync(username, tenant, cancellationToken);

        var role = await _context.Set<Role>().SingleAsync(x => x.Id == roleId);
        user.RemoveRole(role);

        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User> CreateUserAsync(string tenant, string username, string password, string email, 
        string displayName, string avatar, bool isConfirmed, AuthenticationMode authenticationMode, Dictionary<string, string> metadata, CancellationToken cancellationToken)
    {
        tenant = tenant != null ? tenant.ToUpper() : null;

        if (string.IsNullOrEmpty(avatar))
        {
            avatar = AvatarGenerator.GenerateBase64Avatar(displayName);
        }

        var user = new User(tenant, username, email, password, avatar, displayName, authenticationMode, metadata);

        if (isConfirmed)
        {
            user.Confirm();
        }

        _context.Add(user);

        await _context.SaveChangesAsync();

        return user;
    }

    public async Task DeleteUsersFromTenant(string tenant, CancellationToken cancellationToken)
    {
        if (String.IsNullOrEmpty(tenant)) throw new ArgumentNullException(nameof(tenant));

        var users = await _context.Set<User>()
            .Include(x => x.Roles)
            .Include(x => x.Claims)
            .Where(x => x.TenantId == tenant.ToUpper()).ToListAsync();

        _context.RemoveRange(users.SelectMany(x => x.Roles));
        _context.RemoveRange(users.SelectMany(x => x.Claims));
        _context.RemoveRange(users);

        await _context.SaveChangesAsync();
    }

    public async Task ChangePasswordAsync(string tenant, string username, string password, CancellationToken cancellationToken)
    {
        tenant = tenant != null ? tenant.ToUpper() : null;

        var user = await LoadUserEntityAsync(username, tenant, cancellationToken);

        user.ChangePassword(password); 

        await _context.SaveChangesAsync();
    }

    public async Task<User> AppendUserMetadata(string username, string tenant, Dictionary<string, string> metadata, CancellationToken cancellationToken)
    {
        tenant = tenant != null ? tenant.ToUpper() : null;

        var user = await LoadUserEntityAsync(username, tenant, cancellationToken);

        foreach (var kvp in metadata)
        {
            if (user.Metadata.ContainsKey(kvp.Key))
            {
                user.Metadata[kvp.Key] = kvp.Value;
            }
            else
            {
                user.Metadata.Add(kvp.Key, kvp.Value);
            }
        }

        await _context.SaveChangesAsync();
        
        return user;
    }

    public async Task<string[]> GetAllowedTenantsAsync(string username, CancellationToken cancellationToken)
    {
        return await _context.Set<User>().Where(x => x.Username.ToLower() == username.ToLower() && x.TenantId != null).Select(x => x.TenantId).Distinct().ToArrayAsync(cancellationToken);
    }

    public async Task ActivateUserAsync(string tenant, string username, CancellationToken cancellationToken)
    {
        var user = await LoadUserEntityAsync(username, tenant, cancellationToken);
        user.ActivateUser();
        
        await _context.SaveChangesAsync();
    }

    public async Task DeactivateUserAsync(string tenant, string username, CancellationToken cancellationToken)
    {
        var user = await LoadUserEntityAsync(username, tenant, cancellationToken);
        user.DeactivateUser();

        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(string tenant, string username, CancellationToken cancellationToken)
    {
        var user = await GetUserWithDependenciesAsync(username, tenant, cancellationToken);
        _context.RemoveRange(user.Roles);
        _context.RemoveRange(user.Claims);
        _context.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
