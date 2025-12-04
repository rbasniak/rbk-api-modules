// TODO: DONE, REVIEWED

using System.Diagnostics.Metrics;

namespace rbkApiModules.Commons.Core;

public static class EventsMeters
{
    public static readonly Meter Meter = new("PaintingProjects.Events", "1.0.0");

    public static readonly Counter<long> IntegrationOutbox_MessagesProcessed = Meter.CreateCounter<long>("integration_outbox.messages_processed");
    public static readonly Counter<long> IntegrationOutbox_MessagesFailed = Meter.CreateCounter<long>("integration_outbox.messages_failed");
    public static readonly Counter<long> IntegrationOutbox_MessagesPoisoned = Meter.CreateCounter<long>("integration_outbox.messages_poisoned");
    public static readonly Histogram<double> IntegrationOutbox_DispatchDurationMs = Meter.CreateHistogram<double>("integration_outbox.dispatch_duration_ms");
    // Loops should be less than the claim lock duration, so we can use this to track the loop duration
    public static readonly Histogram<double> IntegrationOutbox_LoopDurationMs = Meter.CreateHistogram<double>("integration_outbox.loop_duration_ms");

    public static readonly Counter<long> DomainOutbox_MessagesProcessed = Meter.CreateCounter<long>("domain_outbox.messages_processed");
    public static readonly Counter<long> DomainOutbox_MessagesFailed = Meter.CreateCounter<long>("domain_outbox.messages_failed");
    public static readonly Counter<long> DomainOutbox_MessagesPoisoned = Meter.CreateCounter<long>("domain_outbox.messages_poisoned");
    public static readonly Histogram<double> DomainOutbox_DispatchDurationMs = Meter.CreateHistogram<double>("domain_outbox.dispatch_duration_ms");
    public static readonly Histogram<double> DomainOutbox_LoopDurationMs = Meter.CreateHistogram<double>("domain_outbox.loop_duration_ms");

    public static readonly Counter<long> InboxMessages_Processed = Meter.CreateCounter<long>("inbox.messages_processed");

    public static readonly Counter<long> Dispatcher_RequestsProcessed = Meter.CreateCounter<long>("dispatcher.requests_processed");
    public static readonly Counter<long> Dispatcher_RequestsFailed = Meter.CreateCounter<long>("dispatcher.requests_failed");
    public static readonly Histogram<double> Dispatcher_RequestDurationMs = Meter.CreateHistogram<double>("dispatcher.request_duration_ms");
}