using AutoMapper;
using rbkApiModules.Authentication;
using rbkApiModules.Demo.Models;
using rbkApiModules.Infrastructure.Models;

namespace rbkApiModules.Demo.DataTransfer
{
    public class UsersMapings: Profile
    {
        public UsersMapings()
        {
            CreateMap<ClientUser, SimpleNamedEntity>()
                .ForMember(dto => dto.Id, map => map.MapFrom(entity => entity.Id.ToString()))
                .ForMember(dto => dto.Name, map => map.MapFrom(entity => entity.Username));
            
            CreateMap<BaseUser, SimpleNamedEntity>()
                .ForMember(dto => dto.Id, map => map.MapFrom(entity => entity.Id.ToString()))
                .ForMember(dto => dto.Name, map => map.MapFrom(entity => entity.Username));

            CreateMap<ManagerUser, SimpleNamedEntity>()
                .ForMember(dto => dto.Id, map => map.MapFrom(entity => entity.Id.ToString()))
                .ForMember(dto => dto.Name, map => map.MapFrom(entity => entity.Username));
        }
    }
}
