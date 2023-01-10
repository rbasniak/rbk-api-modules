namespace rbkApiModules.Identity.Core;

public interface IAuthService
{
    Task ConfirmUserAsync(string username, string tenant, CancellationToken cancellation = default);
    Task<User> FindUserAsync(string username, string tenant, CancellationToken cancellation = default);
    Task<User[]> GetAllAsync(string tenant, CancellationToken cancellation = default);
    Task<User> GetUserWithDependenciesAsync(string username, string tenant, CancellationToken cancellation = default);
    Task<User> GetUserFromRefreshtokenAsync(string refreshToken, CancellationToken cancelation = default);
    Task<bool> IsPasswordResetCodeValidAsync(string code, CancellationToken cancelation = default);
    Task<bool> IsRefreshTokenValidAsync(string refreshToken, CancellationToken cancelation = default);
    Task<bool> IsUserConfirmedAsync(string email, string tenant, CancellationToken cancellation = default);
    Task<bool> IsUserRegisteredAsync(string email, string tenant, CancellationToken cancelation = default);
    Task<User> RedefinePasswordAsync(string resetPasswordCode, string password, CancellationToken cancellation = default);
    Task<bool> RefreshTokenExistsOnDatabaseAsync(string refreshToken, CancellationToken cancelation = default);
    Task RequestPasswordResetAsync(string email, string tenant, CancellationToken cancellation = default);
    Task UpdateRefreshTokenAsync(string username, string tenant, string refreshToken, double duration, CancellationToken cancellation = default);
    Task RefreshLastLogin(string username, string tenant, CancellationToken cancellation = default);
    Task<User[]> GetAllWithRoleAsync(string userTenant, string roleTenant, string roleName, CancellationToken cancellation = default);
    Task<User> ReplaceRoles(string username, string tenant, Guid[] roleIds, CancellationToken cancellation);
    Task<User> AddRole(string username, string tenant, Guid roleId, CancellationToken cancellation);
    Task<User> RemoveRole(string username, string tenant, Guid roleId, CancellationToken cancellation);
    Task<User> CreateUserAsync(string tenant, string username, string password, string email, string displayName, string avatar, bool isConfirmed, CancellationToken cancellation = default);
    Task DeleteUsersFromTenant(string tenant, CancellationToken cancellation = default);
}
