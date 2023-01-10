using AutoMapper;
using rbkApiModules.Commons.Core;

namespace Demo1.DataTransfer;

public class BlogMappings: Profile
{
    public BlogMappings()
    {
        CreateMap<Models.Domain.Demo.Blog, SimpleNamedEntity>()
            .ForMember(dto => dto.Id, options => options.MapFrom(entity => entity.Id))
            .ForMember(dto => dto.Name, options => options.MapFrom(entity => entity.Title));
    }
}