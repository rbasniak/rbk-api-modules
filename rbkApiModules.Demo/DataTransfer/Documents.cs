using AutoMapper;
using rbkApiModules.Infrastructure.Models;
using System.Reflection.Metadata;

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
            CreateMap<Document, DocumentDetails>();
        }
    }
}
