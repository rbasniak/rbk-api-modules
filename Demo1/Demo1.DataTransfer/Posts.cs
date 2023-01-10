using AutoMapper;
using Demo1.Models.Read;
using rbkApiModules.Commons.Core;

namespace Demo1.DataTransfer;

public class PostDetails
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public SimpleNamedEntity Author { get; set; }
    public SimpleNamedEntity Blog { get; set; }
}

public class PostMappings: Profile
{
    public PostMappings()
    {
        CreateMap<Post, PostDetails>();
    }
}