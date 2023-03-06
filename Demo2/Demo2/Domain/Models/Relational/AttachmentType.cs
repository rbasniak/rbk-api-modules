using rbkApiModules.Commons.Core;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace GCAB.Models.Domain
{
    public class AttachmentType: BaseEntity
    {
        private readonly HashSet<Attachment> _attachments;
        private readonly HashSet<EvidenceAttachment> _evidenceAttachments;

        public AttachmentType(string name, string extension)
        {
            _attachments = new HashSet<Attachment>();
            _evidenceAttachments = new HashSet<EvidenceAttachment>();

            Name = name;
            Extension = extension;
        }

        public virtual string Name { get; set; }

        public virtual string Extension { get; set; }

        public virtual IEnumerable<Attachment> Attachments => _attachments?.ToList();

        public virtual IEnumerable<EvidenceAttachment> EvidenceAttachments => _evidenceAttachments?.ToList();
    }
}
