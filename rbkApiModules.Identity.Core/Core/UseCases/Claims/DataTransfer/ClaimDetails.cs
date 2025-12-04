using rbkApiModules.Commons.Core.Abstractions;

namespace rbkApiModules.Identity.Core;

public class ClaimDetails  
{
    public required string Id { get; set; } = string.Empty;
    public required string Description { get; set; } = string.Empty;
    public required string Identification { get; set; } = string.Empty;
    public required bool IsProtected { get; set; }

    public static ClaimDetails FromModel(Claim model)
    {
        return new ClaimDetails
        {
            Id = model.Id.ToString(),
            Description = model.Description,
            Identification = model.Identification,
            IsProtected = model.IsProtected
        };
    }
}

public class ClaimOverride
{
    public required EntityReference Claim { get; set; }
    public required string Identification { get; set; } = string.Empty;
    public required EnumReference Access { get; set; }
    public required bool IsProtected { get; set; }

    public static ClaimOverride FromModel(UserToClaim model)
    {
        return new ClaimOverride
        {
            Claim = new EntityReference(model.Claim.Id, model.Claim.Description),
            Identification = model.Claim.Identification,
            Access = new EnumReference(model.Access),
            IsProtected = model.Claim.IsProtected
        };
    }
} 