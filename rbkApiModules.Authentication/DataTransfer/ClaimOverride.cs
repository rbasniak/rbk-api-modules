using AutoMapper;
using rbkApiModules.Infrastructure.Models;

namespace rbkApiModules.Authentication
{
    public class ClaimOverride
    {
        public SimpleNamedEntity Claim { get; set; }
        public string Access { get; set; }
    }

    public class ClaimMappings : Profile
    {
        public ClaimMappings()
        {
            CreateMap<Claim, SimpleNamedEntity>()
                .ForMember(dto => dto.Name, map => map.MapFrom(entity => entity.Description));
        }
    }
}
