using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace rbkApiModules.Demo.Controllers
{
    public class HomeController : Controller
    {
        private Random _random = new Random();

        [HttpGet("/sse1")]
        public async Task SSE()
        {
            await HttpContext.SSEInitAsync();
            Thread.Sleep(_random.Next(50, 3000));
            await HttpContext.SSESendEventAsync(
                new SSEEvent("myEvent", new { Time = DateTime.Now.ToString("hh:mm:ss"), Foo = "foo", Bar = "Bar" })
                {
                    Id = "myId",
                    Retry = 10
                }
            );
        }

        [HttpGet("sse2")]
        public async Task Get()
        {
            var response = Response;
            response.Headers.Add("Content-Type", "text/event-stream");

            for (var i = 0; true; ++i)
            {
                await response.WriteAsync($"data: Controller {i} at {DateTime.Now}\r\r");

                response.Body.Flush();
                await Task.Delay(_random.Next(50, 3000));
            }
        }
    }

    #region Sample 1
    public static class SSEHttpContextExtensions
    {
        public static async Task SSESendCommentAsync(this HttpContext ctx, string comment)
        {
            foreach (var line in comment.Split('\n'))
            {
                await ctx.Response.WriteAsync(": " + line + "\n");
            }

            await ctx.Response.WriteAsync("\n");
            await ctx.Response.Body.FlushAsync();
        }

        public static async Task SSEInitAsync(this HttpContext ctx)
        {
            ctx.Response.Headers.Add("Cache-Control", "no-cache");
            ctx.Response.Headers.Add("Content-Type", "text/event-stream");
            await ctx.Response.Body.FlushAsync();
        }

        public static async Task SSESendDataAsync(this HttpContext ctx, string data)
        {
            foreach (var line in data.Split('\n'))
            {
                await ctx.Response.WriteAsync("data: " + line + "\n");
            }

            await ctx.Response.WriteAsync("\n");
            await ctx.Response.Body.FlushAsync();
        }

        public static async Task SSESendEventAsync(this HttpContext ctx, SSEEvent e)
        {
            if (String.IsNullOrWhiteSpace(e.Id) is false)
            {
                await ctx.Response.WriteAsync("id: " + e.Id + "\n");
            }

            if (e.Retry is not null)
            {
                await ctx.Response.WriteAsync("retry: " + e.Retry + "\n");
            }

            await ctx.Response.WriteAsync("event: " + e.Name + "\n");

            var lines = e.Data switch
            {
                null => new[] { String.Empty },
                string s => s.Split('\n').ToArray(),
                _ => new[] { JsonSerializer.Serialize(e.Data) }
            };

            foreach (var line in lines)
            {
                await ctx.Response.WriteAsync("data: " + line + "\n");
            }

            await ctx.Response.WriteAsync("\n");
            await ctx.Response.Body.FlushAsync();
        }
    }

    public class SSEEvent
    {
        public string Name { get; set; }
        public object Data { get; set; }
        public string Id { get; set; }
        public int? Retry { get; set; }

        public SSEEvent(string name, object data)
        {
            Name = name;
            Data = data;
        }
    }
    #endregion

    #region Sample 2
    public class ServerSentEventsClient
    {
        private readonly HttpResponse _response;
        internal ServerSentEventsClient(HttpResponse response)
        {
            _response = response;
        }

        public Task SendEventAsync(ServerSentEvent serverSentEvent)
        {
            return _response.WriteSseEventAsync(serverSentEvent);
        }
    }

    public class ServerSentEventsService
    {
        private readonly ConcurrentDictionary<Guid, ServerSentEventsClient> _clients = new ConcurrentDictionary<Guid, ServerSentEventsClient>();

        internal Guid AddClient(ServerSentEventsClient client)
        {
            Guid clientId = Guid.NewGuid();

            _clients.TryAdd(clientId, client);

            return clientId;
        }

        internal void RemoveClient(Guid clientId)
        {
            ServerSentEventsClient client;
            _clients.TryRemove(clientId, out client);
        }

        public Task SendEventAsync(ServerSentEvent serverSentEvent)
        {
            List<Task> clientsTasks = new List<Task>();

            foreach (ServerSentEventsClient client in _clients.Values)
            {
                clientsTasks.Add(client.SendEventAsync(serverSentEvent));
            }
            return Task.WhenAll(clientsTasks);
        }
    }

    public class ServerSentEventsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ServerSentEventsService _serverSentEventsService;

        public ServerSentEventsMiddleware(RequestDelegate next, ServerSentEventsService serverSentEventsService)
        {
            _next = next;
            _serverSentEventsService = serverSentEventsService;
        }

        public Task Invoke(HttpContext context)
        {
            if (context.Request.Headers["Accept"] == "text/event-stream")
            {
                context.Response.ContentType = "text/event-stream";
                context.Response.Body.Flush();

                ServerSentEventsClient client = new ServerSentEventsClient(context.Response);
                Guid clientId = _serverSentEventsService.AddClient(client);

                context.RequestAborted.WaitHandle.WaitOne();

                _serverSentEventsService.RemoveClient(clientId);

                return Task.FromResult(true);
            }
            else
            {
                return _next(context);
            }
        }
    }

    public class ServerSentEvent
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public IList<string> Data { get; set; }
    }

    internal static class ServerSentEventsHelper
    {
        internal static async Task WriteSseEventAsync(this HttpResponse response, ServerSentEvent serverSentEvent)
        {
            if (!String.IsNullOrWhiteSpace(serverSentEvent.Id))
                await response.WriteSseEventFieldAsync("id", serverSentEvent.Id);
            
            if (!String.IsNullOrWhiteSpace(serverSentEvent.Type))
                await response.WriteSseEventFieldAsync("event", serverSentEvent.Type);
            
            if (serverSentEvent.Data != null)
            {
                foreach (string data in serverSentEvent.Data)
                    await response.WriteSseEventFieldAsync("data", data);
            }
            
            await response.WriteSseEventBoundaryAsync();
            response.Body.Flush();
        }

        private static Task WriteSseEventFieldAsync(this HttpResponse response, string field, string data)
        {
            return response.WriteAsync($"{field}: {data}\n");
        }

        private static Task WriteSseEventBoundaryAsync(this HttpResponse response)
        {
            return response.WriteAsync("\n");
        }
    }
    #endregion
}
