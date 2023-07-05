using AutoMapper;
using rbkApiModules.Commons.Core;

namespace Demo3.Models
{
    public class ProjectDto
    {
        public class Details : BaseDataTransferObject
        {
            public string Name { get; set; }
            public string Code { get; set; }
            public string Mdb { get; set; }
        }
    }

    public class ProjectMappings : Profile
    {
        public ProjectMappings()
        {
            CreateMap<Project, ProjectDto.Details>();
        }
    }
}
