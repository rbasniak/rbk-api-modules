using AutoMapper;
using rbkApiModules.Demo.Models;
using rbkApiModules.Infrastructure.Models;

namespace rbkApiModules.Demo.DataTransfer
{
    public class UsersMapings: Profile
    {
        public UsersMapings()
        {
            CreateMap<ClientUser, SimpleNamedEntity>()
                .ForMember(dto => dto.Name, map => map.MapFrom(entity => entity.Username));
        }
    }
}
