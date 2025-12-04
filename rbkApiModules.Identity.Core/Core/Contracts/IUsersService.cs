namespace rbkApiModules.Identity.Core;

public interface IAuthService
{
    Task ConfirmUserAsync(string username, string tenant, CancellationToken cancellationToken);
    Task<User> FindUserAsync(string username, string tenant, CancellationToken cancellationToken);
    Task<User[]> GetAllAsync(string tenant, CancellationToken cancellationToken);
    Task<User> GetUserWithDependenciesAsync(string username, string tenant, CancellationToken cancellationToken);
    Task<User> GetUserFromRefreshtokenAsync(string refreshToken, CancellationToken cancellationToken);
    Task<bool> IsPasswordResetCodeValidAsync(string code, CancellationToken cancellationToken);
    Task<bool> IsRefreshTokenValidAsync(string refreshToken, CancellationToken cancellationToken);
    Task<bool> IsUserConfirmedAsync(string email, string tenant, CancellationToken cancellationToken);
    Task<bool> IsUserRegisteredAsync(string email, string tenant, CancellationToken cancellationToken);
    Task<User> RedefinePasswordAsync(string resetPasswordCode, string password, CancellationToken cancellationToken);
    Task ChangePasswordAsync(string tenant, string username, string password, CancellationToken cancellationToken);
    Task<bool> RefreshTokenExistsOnDatabaseAsync(string refreshToken, CancellationToken cancellationToken);
    Task RequestPasswordResetAsync(string email, string tenant, CancellationToken cancellationToken);
    Task UpdateRefreshTokenAsync(string username, string tenant, string refreshToken, double duration, CancellationToken cancellationToken);
    Task RefreshLastLogin(string username, string tenant, CancellationToken cancellationToken);
    Task<User[]> GetAllWithRoleAsync(string userTenant, string roleTenant, string roleName, CancellationToken cancellationToken);
    Task<User> ReplaceRoles(string username, string tenant, Guid[] roleIds, CancellationToken cancellationToken);
    Task<User> AddRole(string username, string tenant, Guid roleId, CancellationToken cancellationToken);
    Task<User> RemoveRole(string username, string tenant, Guid roleId, CancellationToken cancellationToken);
    Task<User> CreateUserAsync(string tenant, string username, string password, string email, string displayName, string avatar, bool isConfirmed, AuthenticationMode authenticationMode, Dictionary<string, string> metadata, CancellationToken cancellationToken);
    Task DeleteUsersFromTenant(string tenant, CancellationToken cancellationToken);
    Task<User> AppendUserMetadata(string username, string tenant, Dictionary<string, string> metadata, CancellationToken cancellationToken);
    Task ActivateUserAsync(string tenant, string username, CancellationToken cancellationToken);
    Task DeactivateUserAsync(string tenant, string username, CancellationToken cancellationToken);
    Task DeleteUserAsync(string tenant, string username, CancellationToken cancellationToken);
    Task<string[]> GetAllowedTenantsAsync(string username, CancellationToken cancellationToken);
}
