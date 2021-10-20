using System;
using AutoMapper;
using rbkApiModules.Infrastructure.Models;
using rbkApiModules.Notifications.Models;

namespace rbkApiModules.Notifications
{
    public class NotificationDto : BaseDataTransferObject
    {
        public string Category { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime Date { get; set; }
        public NotificationStatus Status { get; set; }
        public NotificationType Type { get; set; }
        public string Link { get; set; }
        public string Route { get; set; }
        public string SearchData { get; set; }
    }

    public class NotificationsMappings : Profile
    {
        public NotificationsMappings()
        {
            CreateMap<Notification, NotificationDto>()
                .ForMember(dto => dto.SearchData, map => map.MapFrom(entity => $"{entity.Title}{entity.Body}{entity.Category}"));
        }
    }
}
