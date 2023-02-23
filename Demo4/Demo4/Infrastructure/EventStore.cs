using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Demo4.Infrastructure
{
    public class EventStore
    {

    }

    public class Event
    {
        protected Event()
        {

        }

        public Event(Guid streamId, int currentVersion, Event @event)
        {
            Id = Guid.NewGuid();
            StreamId = streamId;
            Version = currentVersion + 1;
            Type = @event.GetType().Name;
            Timestamp = DateTime.UtcNow;
            Data = JsonSerializer.Serialize((object)@event);
        }

        public Guid Id { get; protected set; }
        
        public Guid StreamId { get; protected set; }
        
        public int Version { get; protected set; }

        public string Type { get; protected set; }

        public DateTime Timestamp { get; protected set; }

        [Column(TypeName = "jsonb")]
        public string Data { get; protected set; }
    }

    public class Stream
    {
        public Guid Id { get; protected set; }
        public int Version { get; protected set; }
    }

    public static class Utils
    {
        public static async Task<bool> SaveEvents(DbContext context, Guid streamId, List<Event> events)
        {
            var stream = await context.Set<Stream>().FindAsync(streamId);

            var version = 0;

            if (stream != null)
            {
                if (stream.Version != events[0].Version + 1)
                {
                    throw new Exception("Concurrency exception");
                }
            }

            await context.Set<Event>().AddRangeAsync(events);
            await context.SaveChangesAsync();

            return true;
        }
    }
}
