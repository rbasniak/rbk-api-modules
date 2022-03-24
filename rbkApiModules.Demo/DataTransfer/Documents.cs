using AutoMapper;
using rbkApiModules.Demo.Models.StateMachine;
using rbkApiModules.Infrastructure.Models;

namespace rbkApiModules.Demo.DataTransfer
{
    public class DocumentDetails: BaseDataTransferObject
    {
        public SimpleNamedEntity Status { get; set; }
    }

    public class DocumentsMapings: Profile
    {
        public DocumentsMapings()
        {
            CreateMap<Document, DocumentDetails>()
                .ForMember(dto => dto.Status, map => map.MapFrom(entity => new SimpleNamedEntity(entity.State.Id.ToString(), entity.State.Name)));
        }
    }
}
