using AutoMapper;
using rbkApiModules.Commons.Core;
using System.Diagnostics;

namespace rbkApiModules.Identity.Core;

public class Roles
{
    [DebuggerDisplay("{Name}")]
    public class Details : BaseDataTransferObject
    {
        public string Name { get; set; }
        public SimpleNamedEntity[] Claims { get; set; }
        public bool IsApplicationWide { get; set; }
    }
}

public class RoleMappings : Profile
{
    public RoleMappings()
    {
        CreateMap<Role, SimpleNamedEntity>();

        CreateMap<Role, Roles.Details>();

        CreateMap<RoleToClaim, SimpleNamedEntity>()
            .ForMember(dto => dto.Id, map => map.MapFrom(entity => entity.Claim.Id))
            .ForMember(dto => dto.Name, map => map.MapFrom(entity => entity.Claim.Description));

        CreateMap<UserToRole, SimpleNamedEntity>()
            .ForMember(dto => dto.Id, map => map.MapFrom(entity => entity.RoleId))
            .ForMember(dto => dto.Name, map => map.MapFrom(entity => entity.Role.Name));

    }
}
