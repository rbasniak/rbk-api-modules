﻿using rbkApiModules.Commons.Core;
using System;
using System.ComponentModel.DataAnnotations;

namespace GCAB.Models.Domain
{
    public class EvidenceAttachment : BaseEntity
    {
        public EvidenceAttachment(string name, AttachmentType type, long size, string path, string filename, string commentary, ChangeRequest changeRequest)
        {
            Name = name;
            Type = type;
            Size = size;
            Path = path;
            Filename = filename;
            Commentary = commentary;
            ChangeRequest = changeRequest;
            AdditionDate = DateTime.Now;
        }

        public virtual string Name { get; set; }

        public virtual Guid TypeId { get; set; }
        public virtual AttachmentType Type { get; set; }

        public virtual Guid ChangeRequestId { get; set; }
        public virtual ChangeRequest ChangeRequest { get; set; }

        public virtual long Size { get; set; }

        public virtual DateTime AdditionDate { get; set; }

        public virtual string Path { get; set; }

        public virtual string Filename { get; set; }

        public virtual string Commentary { get; set; }
    }
}
