using AutoMapper;
using rbkApiModules.Infrastructure.Models;

namespace rbkApiModules.Payment.SqlServer
{
    public class PlanDto
    {
        public class Details : BaseDataTransferObject
        {
            public string Name { get; set; }
            public bool IsActive { get; set; }
            public int Duration { get; set; }
            public double Price { get; set; }
            public string PaypalId { get; set; }
            public bool IsDefault { get; set; }
        }
    }

    public class PlanMappings : Profile
    {
        public PlanMappings()
        {
            CreateMap<Plan, PlanDto.Details>();

            CreateMap<Plan, SimpleNamedEntity>();
        }
    }
}
