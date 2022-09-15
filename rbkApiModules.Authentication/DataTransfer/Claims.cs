using AutoMapper;
using rbkApiModules.Infrastructure.Models;
using rbkApiModules.Utilities;

namespace rbkApiModules.Authentication
{
    public class ClaimDetails : BaseDataTransferObject
    {
        public string Description { get; set; }
        public string Name { get; set; }
    }

    public class ClaimOverride
    {
        public SimpleNamedEntity Claim { get; set; }
        public string Name { get; set; }
        public SimpleNamedEntity<int> Access { get; set; }
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
                .ForMember(dto => dto.Name, map => map.MapFrom(entity => entity.Claim.Name))
                .ForMember(dto => dto.Claim, map => map.MapFrom(entity => new SimpleNamedEntity { Id = entity.Claim.Id.ToString(), Name = entity.Claim.Description }))
                .ForMember(dto => dto.Access, map => map.MapFrom(entity => new SimpleNamedEntity<int> { Id = (int)entity.Access, Name = entity.Access.GetDescription() }));
        }
    }
}
