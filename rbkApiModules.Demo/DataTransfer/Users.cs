using AutoMapper;
using rbkApiModules.Demo.Models;
using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace rbkApiModules.Demo.DataTransfer
{
    public class UsersMapings: Profile
    {
        public UsersMapings()
        {
            CreateMap<User, SimpleNamedEntity>()
                .ForMember(dto => dto.Name, map => map.MapFrom(entity => entity.Username));
        }
    }
}
