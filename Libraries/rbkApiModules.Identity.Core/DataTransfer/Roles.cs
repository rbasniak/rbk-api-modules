using AutoMapper;
using rbkApiModules.Commons.Core;
using System.ComponentModel;
using System.Diagnostics;

namespace rbkApiModules.Identity.Core.DataTransfer.Roles;

[DebuggerDisplay("{Name}")]
public class RoleDetails : BaseDataTransferObject
{
    public string Name { get; set; }
    public SimpleNamedEntity[] Claims { get; set; }
    public RoleMode Mode { get; set; }
    public RoleSource Source { get; set; }
}

public enum RoleMode
{
    [Description("Normal")]
    Normal = 1,

    [Description("Sobrescrita")]
    Overwritten = 2
}

public enum RoleSource
{
    [Description("Global")]
    Global = 1,

    [Description("Local")]
    Local = 2
}

public class RoleMappings : Profile
{
    public RoleMappings()
    {
        CreateMap<Role, SimpleNamedEntity>();

        CreateMap<Role, RoleDetails>()
            .ForMember(dto => dto.Source, map => map.MapFrom(entity => entity.IsApplicationWide ? RoleSource.Global : RoleSource.Local))
            .ForMember(dto => dto.Mode, map => map.MapFrom(entity => entity.HasTenant ? RoleMode.Overwritten : RoleMode.Normal));

        CreateMap<RoleToClaim, SimpleNamedEntity>()
            .ForMember(dto => dto.Id, map => map.MapFrom(entity => entity.Claim.Id))
            .ForMember(dto => dto.Name, map => map.MapFrom(entity => entity.Claim.Description));

        CreateMap<UserToRole, SimpleNamedEntity>()
            .ForMember(dto => dto.Id, map => map.MapFrom(entity => entity.RoleId))
            .ForMember(dto => dto.Name, map => map.MapFrom(entity => entity.Role.Name));

    }
}
