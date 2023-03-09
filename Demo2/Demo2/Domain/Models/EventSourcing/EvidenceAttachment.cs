﻿using rbkApiModules.Commons.Core;
using System;
using System.ComponentModel.DataAnnotations;

namespace Demo2.EventSourcing;

public class EvidenceAttachment : BaseEntity
{
    public EvidenceAttachment()
    {

    }
    public EvidenceAttachment(string name, AttachmentType type, long size, string path, string filename, string commentary)
    {
        Name = name;
        Type = type;
        Size = size;
        Path = path;
        Filename = filename;
        Commentary = commentary;
        AdditionDate = DateTime.Now;
    }

    public virtual string Name { get; set; }

    public virtual Guid TypeId { get; set; }
    public virtual AttachmentType Type { get; set; }

    public virtual long Size { get; set; }

    public virtual DateTime AdditionDate { get; set; }

    public virtual string Path { get; set; }

    public virtual string Filename { get; set; }

    public virtual string Commentary { get; set; }
}
