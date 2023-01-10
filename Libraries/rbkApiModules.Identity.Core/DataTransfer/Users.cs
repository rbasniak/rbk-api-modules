using AutoMapper;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Identity.Core;

public class UserDetails : BaseDataTransferObject
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string DisplayName { get; set; }
    public DateTime? LastLogin { get; set; }
    public string Metadata { get; set; }
    public bool IsConfirmed { get; set; }
    public SimpleNamedEntity[] Roles { get; set; }
}

public class UserMappings : Profile
{
    public UserMappings()
    {
        CreateMap<User, UserDetails>();

    }
}
