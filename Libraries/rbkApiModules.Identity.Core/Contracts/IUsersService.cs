namespace rbkApiModules.Identity.Core;

public interface IAuthService
{
    Task ConfirmUserAsync(string username, string tenant, CancellationToken cancellation);
    Task<User> FindUserAsync(string username, string tenant, CancellationToken cancellation);
    Task<User[]> GetAllAsync(string tenant, CancellationToken cancellation);
    Task<User> GetUserWithDependenciesAsync(string username, string tenant, CancellationToken cancellation);
    Task<User> GetUserFromRefreshtokenAsync(string refreshToken, CancellationToken cancelation);
    Task<bool> IsPasswordResetCodeValidAsync(string code, CancellationToken cancelation);
    Task<bool> IsRefreshTokenValidAsync(string refreshToken, CancellationToken cancelation);
    Task<bool> IsUserConfirmedAsync(string email, string tenant, CancellationToken cancellation);
    Task<bool> IsUserRegisteredAsync(string email, string tenant, CancellationToken cancelation);
    Task<User> RedefinePasswordAsync(string resetPasswordCode, string password, CancellationToken cancellation);
    Task ChangePasswordAsync(string tenant, string username, string password, CancellationToken cancellation);
    Task<bool> RefreshTokenExistsOnDatabaseAsync(string refreshToken, CancellationToken cancelation);
    Task RequestPasswordResetAsync(string email, string tenant, CancellationToken cancellation);
    Task UpdateRefreshTokenAsync(string username, string tenant, string refreshToken, double duration, CancellationToken cancellation);
    Task RefreshLastLogin(string username, string tenant, CancellationToken cancellation);
    Task<User[]> GetAllWithRoleAsync(string userTenant, string roleTenant, string roleName, CancellationToken cancellation);
    Task<User> ReplaceRoles(string username, string tenant, Guid[] roleIds, CancellationToken cancellation);
    Task<User> AddRole(string username, string tenant, Guid roleId, CancellationToken cancellation);
    Task<User> RemoveRole(string username, string tenant, Guid roleId, CancellationToken cancellation);
    Task<User> CreateUserAsync(string tenant, string username, string password, string email, string displayName, string avatar, bool isConfirmed, CancellationToken cancellation);
    Task DeleteUsersFromTenant(string tenant, CancellationToken cancellation);
}
