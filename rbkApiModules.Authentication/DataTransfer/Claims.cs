using AutoMapper;
using rbkApiModules.Infrastructure.Models;

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
        public string Access { get; set; }
    }

    public class ClaimMappings : Profile
    {
        public ClaimMappings()
        {
            CreateMap<Claim, ClaimDetails>();
        }
    }
}
