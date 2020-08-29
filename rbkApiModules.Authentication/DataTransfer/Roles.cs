using AutoMapper;
using rbkApiModules.Infrastructure;
using rbkApiModules.Infrastructure.Models;

namespace rbkApiModules.Authentication
{
    public class Roles
    {
        public class Details : BaseDataTransferObject
        {
            public string Name { get; set; }
            public SimpleNamedEntity[] Claims { get; set; }
        }
    }

    public class RoleMappings : Profile
    {
        public RoleMappings()
        {
            CreateMap<Role, SimpleNamedEntity>();

            CreateMap<Role, Roles.Details>();

            CreateMap<RoleToClaim, SimpleNamedEntity>()
                .ForMember(dto => dto.Id, map => map.MapFrom(entity => entity.ClaimId))
                .ForMember(dto => dto.Name, map => map.MapFrom(entity => entity.Claim.Name));
        }
    }
}
