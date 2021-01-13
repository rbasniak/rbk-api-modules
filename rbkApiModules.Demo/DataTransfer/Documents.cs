using AutoMapper;
using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

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
