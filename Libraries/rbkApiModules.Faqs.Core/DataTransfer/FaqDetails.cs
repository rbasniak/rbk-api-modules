using AutoMapper;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Faqs.Core;

public class FaqDetails : BaseDataTransferObject
{
    public string Question { get; set; }
    public string Answer { get; set; }
    public bool IsGlobal { get; set; }
}

public class FaqMappings : Profile
{
    public FaqMappings()
    {
        CreateMap<Faq, FaqDetails>()
            .ForMember(dto => dto.IsGlobal, map => map.MapFrom(entity => entity.HasNoTenant))
            .ForMember(dto => dto.Id, map => map.MapFrom(entity => entity.Id.ToString().ToLower()));
    }
}
