using rbkApiModules.Commons.Core.Abstractions;

namespace rbkApiModules.Identity.Core.DataTransfer.Users;

public class UserDetails  
{
    public required Guid Id { get; set; }
    public required string Username { get; set; } = string.Empty;
    public required string Email { get; set; } = string.Empty;
    public required string DisplayName { get; set; } = string.Empty;
    public required bool IsActive { get; set; }
    public required DateTime? LastLogin { get; set; }
    public required string Avatar { get; set; } = string.Empty;
    public required bool IsConfirmed { get; set; }
    public required EntityReference[] Roles { get; set; } = [];
    public required ClaimOverride[] OverridedClaims { get; set; } = [];
    public required EntityReference<string>[] Claims { get; set; } = [];
    public required Dictionary<string, string> Metadata { get; set; } = new();

    public static UserDetails FromModel(User model)
    {
        return new UserDetails
        {
            Id = model.Id,
            Username = model.Username,
            Email = model.Email,
            DisplayName = model.DisplayName,
            IsActive = model.IsActive,
            LastLogin = model.LastLogin,
            Avatar = model.Avatar,
            IsConfirmed = model.IsConfirmed,
            Roles = model.Roles.Select(x => x.Role).Select(x => new EntityReference(x.Id, x.Name)).ToArray(),
            OverridedClaims = model.Claims.Select(x => ClaimOverride.FromModel(x)).ToArray(),
            Claims = model.GetAccessClaims().Select(x => new EntityReference<string>(x.Identification, x.Description)).ToArray(),
            Metadata = model.Metadata,
        };
    }
}

public class ClaimOverride
{
    public required EntityReference Claim { get; set; }
    public required EnumReference Access { get; set; }

    public static ClaimOverride FromModel(UserToClaim model)
    {
        return new ClaimOverride
        {
            Claim = new EntityReference(model.Claim.Id, model.Claim.Description),
            Access = new EnumReference(model.Access)
        };
    }
} 