using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo2.Domain.Events.Infrastructure;

public class EventStoreDao
{
    public Guid Id { get; set; }
    public string Data { get; set; }
    public int Version { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    [Description("ignore")]
    public int Sequence { get; set; }

    public string Name { get; set; }
    public string Aggregate { get; set; }
    public string AggregateId { get; set; }
}