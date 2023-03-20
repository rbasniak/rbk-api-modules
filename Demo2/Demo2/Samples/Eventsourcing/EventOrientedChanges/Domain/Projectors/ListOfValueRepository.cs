using Demo2.Samples.Relational.Domain.Models;

namespace Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Projectors
{
    public class ListOfValuesRepository
    {
        public bool IsInitialized { get; set; }

        public Dictionary<Guid, ChangeRequestPriority> ChangeRequestPriorities { get; set; }
        public Dictionary<Guid, ChangeRequestSource> ChangeRequestSources { get; set; }
        public Dictionary<Guid, ChangeRequestType> ChangeRequestTypes { get; set; }
        public Dictionary<Guid, DocumentCategory> DocumentCategories { get; set; }
        public Dictionary<Guid, AttachmentType> AttachmentTypes { get; set; }
        public Dictionary<Guid, FicCategory> FicCategories { get; set; }
        public Dictionary<Guid, Discipline> Disciplines { get; set; }
        public Dictionary<Guid, Platform> Platforms { get; set; }
        public Dictionary<Guid, Un> Uns { get; set; }
    }
}
