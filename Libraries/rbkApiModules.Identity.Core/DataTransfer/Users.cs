﻿using AutoMapper;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Identity.Core.DataTransfer.Users;

public class UserDetails : BaseDataTransferObject
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string DisplayName { get; set; }
    public DateTime? LastLogin { get; set; }
    public string Metadata { get; set; }
    public bool IsConfirmed { get; set; }
    public SimpleNamedEntity[] Roles { get; set; }
    public ClaimOverride[] OverridedClaims { get; set; }
    public SimpleNamedEntity[] Claims { get; set; }
}

public class ClaimOverride
{
    public SimpleNamedEntity Claim { get; set; }
    public SimpleNamedEntity<int> Access { get; set; }
}

public class UserMappings : Profile
{
    public UserMappings()
    {
        CreateMap<User, UserDetails>()
            .ForMember(dto => dto.OverridedClaims, map => map.MapFrom(entity => entity.Claims.Select(x => new ClaimOverride
            {
                Claim = new SimpleNamedEntity(x.Claim.Identification, x.Claim.Description),
                Access = new SimpleNamedEntity<int>((int)x.Access, x.Access.GetDescription())
            })))
            .ForMember(dto => dto.Claims, map => map.MapFrom(entity => entity.GetAccessClaims().Select(x => new SimpleNamedEntity(x.Identification, x.Description))));

        CreateMap<UserToClaim, ClaimOverride>()
            .ForMember(dto => dto.Claim, map => map.MapFrom(entity => new SimpleNamedEntity(entity.Claim.Identification, entity.Claim.Description)))
            .ForMember(dto => dto.Access, map => map.MapFrom(entity => new SimpleNamedEntity<int> { Id = (int)entity.Access, Name = entity.Access.GetDescription() }));
    }
}
