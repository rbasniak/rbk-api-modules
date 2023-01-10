using AutoMapper;

namespace Demo1.Models.Read.Mappings;

public class PostMappings : Profile
{
    public PostMappings()
    {
        CreateMap<Domain.Demo.Post, Post>();
    }
}