using AutoMapper;

namespace Demo1.Models.Read.Mappings;

public class BlogMappings : Profile
{
    public BlogMappings()
    {
        CreateMap<Domain.Demo.Blog, Blog>();
    }
}