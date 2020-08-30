using AutoMapper;
using rbkApiModules.Infrastructure;
using rbkApiModules.Infrastructure.Models;

namespace rbkApiModules.Authentication
{
    public class Users
    {
        public class ListItem : BaseDataTransferObject
        {
            public string Username { get; set; }
        }

        public class Details : BaseDataTransferObject
        {
            public string Username { get; set; }
            public ClaimOverride[] Claims { get; set; }
            public SimpleNamedEntity[] Roles { get; set; }
        }
    }

    public class UserMappings : Profile
    {
        public UserMappings()
        {
            CreateMap<BaseUser, SimpleNamedEntity>()
                .ForMember(dto => dto.Name, map => map.MapFrom(entity => entity.Username));

            CreateMap<BaseUser, Users.ListItem>();

            CreateMap<BaseUser, Users.Details>()
                .ForMember(dto => dto.Id, map => map.MapFrom(entity => entity.Id))
                .ForMember(dto => dto.Username, map => map.MapFrom(entity => entity.Username));

            CreateMap<UserToRole, SimpleNamedEntity>()
                .ForMember(dto => dto.Id, map => map.MapFrom(entity => entity.RoleId))
                .ForMember(dto => dto.Name, map => map.MapFrom(entity => entity.Role.Name));

            CreateMap<UserToClaim, ClaimOverride>()
                .ForMember(dto => dto.Claim, map => map.MapFrom(entity => new SimpleNamedEntity() { Id = entity.ClaimId.ToString(), Name = entity.Claim.Name }))
                .ForMember(dto => dto.Access, map => map.MapFrom(entity => entity.Access.ToString()));
        }
    }
}
