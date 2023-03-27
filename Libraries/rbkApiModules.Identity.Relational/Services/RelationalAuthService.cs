using Microsoft.EntityFrameworkCore;
using rbkApiModules.Identity.Core;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Relational.CQRS;
using rbkApiModules.Commons.Relational;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core.Utilities.Localization;

namespace rbkApiModules.Identity.Relational;

public class RelationalAuthService: IAuthService
{
    private readonly DbContext _context;
    private readonly ILocalizationService _localization;

    public RelationalAuthService(IEnumerable<DbContext> contexts, ILocalizationService localization)
    {
        _context = contexts.GetDefaultContext();
        _localization = localization;
    }

    public async Task ConfirmUserAsync(string username, string tenant, CancellationToken cancellation = default)
    {
        tenant = tenant != null ? tenant.ToUpper() : null;

        var user = await LoadUserEntityAsync(username, tenant, cancellation);

        user.Confirm();

        await _context.SaveChangesAsync(cancellation);
    }

    public async Task<User> FindUserAsync(string username, string tenant, CancellationToken cancellation = default)
    {
        tenant = tenant != null ? tenant.ToUpper() : null;

        var user = await LoadUserEntityAsync(username, tenant, cancellation);
        return user;
    }

    public async Task<User> GetUserWithDependenciesAsync(string username, string tenant, CancellationToken cancellation = default)
    {
        tenant = tenant != null ? tenant.ToUpper() : null;

        var user = await _context.Set<User>()
            .Include(x => x.Roles)
                .ThenInclude(x => x.Role)
                    .ThenInclude(x => x.Claims)
                        .ThenInclude(x => x.Claim)
            .Include(x => x.Claims)
                .ThenInclude(x => x.Claim)
            .SingleAsync(x => (x.Username.ToLower() == username.ToLower() || x.Email.ToLower() == username) && x.TenantId == tenant, cancellation);

        return user;
    }

    public async Task UpdateRefreshTokenAsync(string username, string tenant, string refreshToken, double duration, CancellationToken cancellation = default)
    {
        tenant = tenant != null ? tenant.ToUpper() : null;

        var user = await LoadUserEntityAsync(username, tenant, cancellation);

        user.SetRefreshToken(refreshToken, duration);

        await _context.SaveChangesAsync(cancellation);
    }

    private async Task<User> LoadUserEntityAsync(string username, string tenant, CancellationToken cancellation = default)
    {
        tenant = tenant != null ? tenant.ToUpper() : null;

        return  await _context.Set<User>()
            .SingleOrDefaultAsync(x => (x.Username.ToLower() == username.ToLower() || x.Email.ToLower() == username.ToLower()) && x.TenantId == tenant, cancellation);
    }

    public async Task<User> RedefinePasswordAsync(string resetPasswordCode, string password, CancellationToken cancellation = default)
    {
        var user = await _context.Set<User>()
            .Include(x => x.PasswordRedefineCode)
            .FirstOrDefaultAsync(x => x.PasswordRedefineCode.Hash == resetPasswordCode, cancellation);

        if (user == null) throw new SafeException(_localization.GetValue(AuthenticationMessages.Erros.CouldNotFindTheUserAssociatedWithThePasswordResetCode));

        user.ChangePassword(password);

        await _context.SaveChangesAsync(cancellation);

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

    public async Task<bool> IsRefreshTokenValidAsync(string refreshToken, CancellationToken cancellation = default)
    {
        var user = await _context.Set<User>()
            .SingleAsync(x => x.RefreshToken == refreshToken, cancellation);

        return user.RefreshTokenValidity > DateTime.UtcNow;
    }

    public async Task<bool> RefreshTokenExistsOnDatabaseAsync(string refreshToken, CancellationToken cancellation = default)
    {
        return await _context.Set<User>().AnyAsync(x => x.RefreshToken == refreshToken, cancellation);
    }

    public async Task<User> GetUserFromRefreshtokenAsync(string refreshToken, CancellationToken cancellation = default)
    {
        var user = await _context.Set<User>()
                .Include(x => x.Roles)
                    .ThenInclude(x => x.Role)
                        .ThenInclude(x => x.Claims)
                            .ThenInclude(x => x.Claim)
                .Include(x => x.Claims)
                    .ThenInclude(x => x.Claim)
                .SingleOrDefaultAsync(x => x.RefreshToken == refreshToken && x.RefreshTokenValidity > DateTime.UtcNow, cancellation);

        return user;
    }

    public async Task<bool> IsUserRegisteredAsync(string email, string tenant, CancellationToken cancellation = default)
    {
        tenant = tenant != null ? tenant.ToUpper() : null;

        return await _context.Set<User>().AnyAsync(x => x.Email == email && x.TenantId == tenant, cancellation);
    }

    public async Task<bool> IsUserConfirmedAsync(string email, string tenant, CancellationToken cancellation = default)
    {
        tenant = tenant != null ? tenant.ToUpper() : null;

        return await _context.Set<User>().AnyAsync(x => x.Email.ToLower() == email.ToLower() && x.IsConfirmed && x.TenantId == tenant, cancellation);
    }

    public async Task RequestPasswordResetAsync(string email, string tenant, CancellationToken cancellation = default)
    {
        tenant = tenant != null ? tenant.ToUpper() : null;

        var user = await LoadUserEntityAsync(email, tenant, cancellation);

        user.SetPasswordRedefineCode();

        await _context.SaveChangesAsync();
    }

    public async Task RefreshLastLogin(string username, string tenant, CancellationToken cancellation = default)
    {
        tenant = tenant != null ? tenant.ToUpper() : null;

        var user = await LoadUserEntityAsync(username, tenant, cancellation);

        user.RefreshLastLogin();

        await _context.SaveChangesAsync();
    }

    public async Task<User[]> GetAllAsync(string tenant, CancellationToken cancellation = default)
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
            .ToArrayAsync(cancellation);

        return users;
    }

    public async Task<User[]> GetAllWithRoleAsync(string userTenant, string roleTenant, string roleName, CancellationToken cancellation = default)
    {
        User[] users;

        if (String.IsNullOrEmpty(roleTenant))
        {
            users = await _context.Set<User>()
                .Include(x => x.Roles)
                    .ThenInclude(x => x.Role)
                .OrderBy(x => x.Username)
                .Where(x => x.TenantId != null && x.TenantId.ToLower() == userTenant.ToLower() && x.Roles.Any(x => x.Role.Name.ToLower() == roleName.ToLower() && x.Role.TenantId == null))
                .ToArrayAsync(cancellation);
        }
        else
        {
            users = await _context.Set<User>()
                .Include(x => x.Roles)
                    .ThenInclude(x => x.Role)
                .OrderBy(x => x.Username)
                .Where(x => x.TenantId != null && x.TenantId.ToLower() == userTenant.ToLower() && x.Roles.Any(x => x.Role.Name.ToLower() == roleName.ToLower() && x.Role.TenantId != null && x.Role.TenantId.ToLower() == roleTenant.ToLower()))
                .ToArrayAsync(cancellation);
        }

        return users;
    }

    public async Task<User> ReplaceRoles(string username, string tenant, Guid[] roleIds, CancellationToken cancellation)
    {
        tenant = tenant != null ? tenant.ToUpper() : null;

        var user = await GetUserWithDependenciesAsync(username, tenant, cancellation);

        _context.RemoveRange(user.Roles);

        await _context.SaveChangesAsync();

        foreach (var roleId in roleIds)
        {
            var role = await _context.Set<Role>().Include(x => x.Claims).ThenInclude(x => x.Claim).SingleAsync(c => c.Id == roleId);
            user.AddRole(role);
        }

        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User> AddRole(string username, string tenant, Guid roleId, CancellationToken cancellation)
    {
        tenant = tenant != null ? tenant.ToUpper() : null;

        var user = await LoadUserEntityAsync(username, tenant, cancellation);

        var role = await _context.Set<Role>().SingleAsync(x => x.Id == roleId);
        user.AddRole(role);

        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User> RemoveRole(string username, string tenant, Guid roleId, CancellationToken cancellation)
    {
        tenant = tenant != null ? tenant.ToUpper() : null;

        var user = await LoadUserEntityAsync(username, tenant, cancellation);

        var role = await _context.Set<Role>().SingleAsync(x => x.Id == roleId);
        user.RemoveRole(role);

        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User> CreateUserAsync(string tenant, string username, string password, string email, string displayName, string avatarPath, bool isConfirmed, CancellationToken cancellation)
    {
        tenant = tenant != null ? tenant.ToUpper() : null;

        var user = new User(tenant, username, email, password, avatarPath, displayName);

        if (isConfirmed)
        {
            user.Confirm();
        }

        _context.Add(user);

        await _context.SaveChangesAsync();

        return user;
    }

    public async Task DeleteUsersFromTenant(string tenant, CancellationToken cancellation)
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

    public async Task ChangePasswordAsync(string tenant, string username, string password, CancellationToken cancellation = default)
    {
        tenant = tenant != null ? tenant.ToUpper() : null;

        var user = await LoadUserEntityAsync(username, tenant, cancellation);

        user.ChangePassword(password); 

        await _context.SaveChangesAsync();
    } 
}
