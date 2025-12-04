using Microsoft.Extensions.Logging;

namespace rbkApiModules.Identity.Core;

public interface IAutomaticUserCreator
{
    Task<User> CreateIfAllowedAsync(string username, string tenant, CancellationToken cancellationToken);
}

public class AutomaticUserCreator : IAutomaticUserCreator
{
    private readonly IAuthService _authService;
    private readonly IRolesService _rolesService;
    private readonly RbkAuthenticationOptions _authOptions;
    private readonly ILocalizationService _localizationService;
    private readonly ICustomUserPostProcessor _customUserPostProcessor;
    private readonly ILogger<AutomaticUserCreator> _logger;

    public AutomaticUserCreator(IAuthService authService, RbkAuthenticationOptions authOptions,
        IEnumerable<ICustomUserPostProcessor> customUserPostProcessors, IRolesService rolesService,
        ILocalizationService localizationService, ILogger<AutomaticUserCreator> logger)
    {
        _authOptions = authOptions;
        _authService = authService;
        _rolesService = rolesService;
        _localizationService = localizationService;
        _customUserPostProcessor = customUserPostProcessors.Count() > 0 ? customUserPostProcessors.First() : null;
        _logger = logger;
    }

    public async Task<User> CreateIfAllowedAsync(string username, string tenant, CancellationToken cancellationToken)
    {
        if (_authOptions._allowUserCreationOnFirstAccess && _authOptions._loginMode == LoginMode.WindowsAuthentication ||
            _authOptions._allowUserCreationOnFirstAccess && _authOptions._loginMode == LoginMode.Custom)
        {
            _logger.LogInformation("User does not exist and the project is setup to allow automatic creation. User will be created now.");

            var userInfo = new UserCustomInformation
            {
                Avatar = AvatarGenerator.GenerateBase64Avatar(username),
                DisplayName = username,
                Email = $"{username.ToLower()}@unknown.com",
                Metadata = new Dictionary<string, string>()
            };

            if (_customUserPostProcessor != null)
            {
                userInfo = await _customUserPostProcessor.GetUserExtraInformationAsync(tenant, username, cancellationToken);
            }

            var authenticationMode = _authOptions._loginMode == LoginMode.Custom ? AuthenticationMode.Custom : AuthenticationMode.Windows;

            var user = await _authService.CreateUserAsync(
                tenant: tenant,
                username: username,
                password: null,
                email: userInfo.Email,
                displayName: userInfo.DisplayName,
                avatar: userInfo.Avatar,
                isConfirmed: true,
                authenticationMode: authenticationMode,
                metadata: userInfo.Metadata,
                cancellationToken);

            var roles = await _rolesService.GetAllAsync(cancellationToken);

            var possibleRoles = roles.Where(x => x.Name.ToLower() == _authOptions._defaultRoleName.ToLower()).ToList();

            if (possibleRoles.Count == 0) throw new ExpectedInternalException(_localizationService.LocalizeString(AuthenticationMessages.Erros.CannotFindDefaultRole));

            var tenantRole = possibleRoles.FirstOrDefault(x => x.TenantId == tenant);
            var masterRole = possibleRoles.FirstOrDefault(x => x.TenantId == null);

            if (tenantRole != null)
            {
                await _authService.AddRole(username, tenant, tenantRole.Id, cancellationToken);
            }
            else
            {
                await _authService.AddRole(username, tenant, masterRole.Id, cancellationToken);
            }

            return user;
        }
        else
        {
            return null;
        }
    }
}