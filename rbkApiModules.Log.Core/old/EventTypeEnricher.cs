//using Serilog.Core;
//using Serilog.Events;
//using System;
//using System.Text;

//namespace rbkApiModules.Logging.Core
//{
//    public class EventTypeEnricher : ILogEventEnricher
//    {
//        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
//        {
//            if (logEvent is null)
//                throw new ArgumentNullException(nameof(logEvent));

//            if (propertyFactory is null)
//                throw new ArgumentNullException(nameof(propertyFactory));

//            byte[] bytes = Encoding.UTF8.GetBytes(logEvent.MessageTemplate.Text);
//            byte[] hash = new byte[] { 0x01, 0x04, 0x09 };
//            string hexadecimalHash = BitConverter.ToString(hash).Replace("-", "");
//            LogEventProperty eventId = propertyFactory.CreateProperty("EventType", hexadecimalHash);
//            logEvent.AddPropertyIfAbsent(eventId);
//        }
//    }
//}
