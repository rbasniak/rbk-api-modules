using AutoMapper;
using rbkApiModules.Infrastructure.Models;

namespace rbkApiModules.Payment.SqlServer
{
    public class TrialKeyDto
    {
        public class Details : BaseDataTransferObject
        {
            public string PlanName { get; set; }
            public int TrialPeriod { get; set; }
        }
    }

    public class TrialKeyMappings : Profile
    {
        public TrialKeyMappings()
        {
            CreateMap<TrialKey, TrialKeyDto.Details>()
                .ForMember(x => x.PlanName, x => x.MapFrom(x => x.Plan.Name));
        }
    }
}
