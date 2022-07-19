using AutoMapper;
using rbkApiModules.Infrastructure.Models;

namespace rbkApiModules.Faqs
{
    public class FaqDetails : BaseDataTransferObject
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }

    public class FaqMappings : Profile
    {
        public FaqMappings()
        {
            CreateMap<Faq, FaqDetails>()
                .ForMember(dto => dto.Id, map => map.MapFrom(entity => entity.Id.ToString()));
        }
    }
}
