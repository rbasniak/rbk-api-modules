using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using rbkApiModules.Infrastructure.MediatR.Core;
using rbkApiModules.Infrastructure.MediatR.SqlServer;
using System;
using System.IO;
using System.Threading.Tasks;

namespace rbkApiModules.Paypal.SqlServer
{
    public class CreateWebhookEvent
    {
        public class Command : IRequest<CommandResponse>
        {
            public IHeaderDictionary EventHeader { get; set; }
            public Stream EventBody { get; set; }
        }

        public class Handler : BaseCommandHandler<Command, DbContext>
        {
            private readonly IPaypalService _paypalService;
            private readonly IPaypalActions _paypalActions;

            public Handler(DbContext context, IHttpContextAccessor httpContextAccessor, IPaypalService paypalService, IPaypalActions paypalActions)
                : base(context, httpContextAccessor)
            {
                _paypalService = paypalService;
                _paypalActions = paypalActions;
            }

            protected override async Task<(Guid? entityId, object result)> ExecuteAsync(Command request)
            {
                var requestBody = string.Empty;
                using (var reader = new StreamReader(request.EventBody))
                {
                    requestBody = reader.ReadToEndAsync().Result;
                }

                var jsonBody = JsonConvert.DeserializeObject<WebhookEventResponse>(requestBody);

                var validationStatus = "";

                try
                {
                    validationStatus = await _paypalService.ValidateWebhookSignature(requestBody, request.EventHeader);
                }
                catch (Exception ex)
                {
                    validationStatus = "ERROR (" + ex.Message + ")";
                }

                var webhookEvent = new WebhookEvent(jsonBody.CreateTime, jsonBody.EventType, requestBody,
                                                    jsonBody.Resource?.Id, jsonBody.Resource?.PlanId,
                                                    jsonBody.Resource?.Subscriber?.EmailAddress, validationStatus);

                await _context.AddAsync(webhookEvent);

                _paypalActions.OnWebhookEventReceived(jsonBody);

                await _context.SaveChangesAsync();

                return (webhookEvent.Id, webhookEvent);
            }
        }
    }
}
