using Demo2.Relational;

namespace Demo2.Domain.Eventsourcing.Projectors
{
    public class ListOfValuesRepository
    {
        public bool IsInitialized { get; set; }

        public IEnumerable<ChangeRequestPriority> ChangeRequestPriorities { get; set; }
        public IEnumerable<ChangeRequestSource> ChangeRequestSources { get; set; }
        public IEnumerable<ChangeRequestType> ChangeRequestTypes { get; set; }
        public IEnumerable<DocumentCategory> DocumentCategories { get; set; }
        public IEnumerable<AttachmentType> AttachmentTypes { get; set; }
        public IEnumerable<FicCategory> FicCategories { get; set; }
        public IEnumerable<Discipline> Disciplines { get; set; }
        public IEnumerable<Platform> Platforms { get; set; }
        public IEnumerable<Un> Uns { get; set; }
    }
}
