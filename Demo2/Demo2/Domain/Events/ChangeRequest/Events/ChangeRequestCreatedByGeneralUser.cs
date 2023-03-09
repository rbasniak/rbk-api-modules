using Demo2.EventSourcing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

public class ChangeRequestCreatedByGeneralUser
{
    public class V1 : DomainEvent
    {
        public V1(string username, Guid changeRequestId, Guid platformId, ChangeRequestType type, ChangeRequestPriority priority, 
            ChangeRequestSource source, string description, string justification, string requestedBy, string createdBy, 
            string sourceNumber) : base(username, changeRequestId)
        {
            ChangeRequestId = changeRequestId;
            PlatformId = platformId;
            Type = type;
            Priority = priority;
            Source = source;
            ChangeRequestDescription = description;
            CreatedBy = createdBy;
            SourceNumber = sourceNumber;
            RequestedBy = requestedBy;
            Justification = justification;
        }

        public Guid ChangeRequestId { get; set; }
        public Guid PlatformId { get; set; }
        public ChangeRequestType Type { get; set; }
        public ChangeRequestPriority Priority { get; set; }
        public ChangeRequestSource Source { get; set; }
        public string ChangeRequestDescription { get; set; }
        public string Justification { get; set; }
        public string RequestedBy { get; set; }
        public string CreatedBy { get; set; }
        public string SourceNumber { get; set; } 
    }
}