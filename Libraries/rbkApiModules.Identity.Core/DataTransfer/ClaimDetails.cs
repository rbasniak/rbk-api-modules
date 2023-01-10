using AutoMapper;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Identity.Core;

public class ClaimDetails : BaseDataTransferObject
{
    public string Description { get; set; }
    public string Identification { get; set; }
    public bool IsProtected { get; set; }
}

public class ClaimOverride
{
    public SimpleNamedEntity Claim { get; set; }
    public string Identification { get; set; }
    public SimpleNamedEntity<int> Access { get; set; }
    public bool IsProtected { get; set; }
}

public class ClaimMappings : Profile
{
    public ClaimMappings()
    {
        CreateMap<Claim, ClaimDetails>();

        CreateMap<UserToClaim, SimpleNamedEntity>()
            .ForMember(dto => dto.Id, map => map.MapFrom(entity => entity.ClaimId))
            .ForMember(dto => dto.Name, map => map.MapFrom(entity => entity.Claim.Description));

        CreateMap<UserToClaim, ClaimOverride>()
            .ForMember(dto => dto.Identification, map => map.MapFrom(entity => entity.Claim.Identification))
            .ForMember(dto => dto.Claim, map => map.MapFrom(entity => new SimpleNamedEntity { Id = entity.Claim.Id.ToString(), Name = entity.Claim.Description }))
            .ForMember(dto => dto.Access, map => map.MapFrom(entity => new SimpleNamedEntity<int> { Id = (int)entity.Access, Name = entity.Access.GetDescription() }))
            .ForMember(dto => dto.IsProtected, map => map.MapFrom(entity => entity.Claim.IsProtected));
    }
}