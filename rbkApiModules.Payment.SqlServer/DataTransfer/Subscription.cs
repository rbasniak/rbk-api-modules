using AutoMapper;
using rbkApiModules.Infrastructure.Models;

namespace rbkApiModules.Payment.SqlServer
{
    public class SubscriptionDto
    {
        public class Details : BaseDataTransferObject
        {
            public string SubscriptionID { get; set; }
        }
    }

    public class SubscriptionMappings : Profile
    {
        public SubscriptionMappings()
        {
            CreateMap<Subscription, SubscriptionDto.Details>();
        }
    }
}
