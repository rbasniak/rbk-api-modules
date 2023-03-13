using rbkApiModules.Commons.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Demo2.EventSourcing;

public class AttachmentTypeInfo 
{
    public AttachmentTypeInfo()
    {

    }
    public AttachmentTypeInfo(AttachmentType type, string extension)
    {
        Id = (int)type;
        Extension = extension;
    }
    public virtual int Id { get; set; }
    public virtual string Extension { get; set; }
}

public enum AttachmentType
{
    [Description("Imagens")]
    Images = 1,

    [Description("Documentos")]
    Documents = 2,

    [Description("Vídeos")]
    Videos = 3,

    [Description("Modelos 3D")]
    TridimensionalModel
}
