using AutoMapper;
using Demo1.Models.Domain.Demo;
using rbkApiModules.Commons.Core;

namespace Demo1.DataTransfer;

public class AuthorMappings: Profile
{
    public AuthorMappings()
    {
        CreateMap<Author, SimpleNamedEntity>();
    }
}