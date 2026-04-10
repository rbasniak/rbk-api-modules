using FluentValidation;
using rbkApiModules.Commons.Core.Features.ApplicationOptions;

namespace Demo1.UseCases.Commands;

public class GetSettings : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/options", async (IDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var result = await dispatcher.SendAsync(new Request(), cancellationToken);
            return Results.Ok(result.Data);
        })
        .WithName("Get Application Options")
        .WithTags("Options");
    }

    public class Request : ICommand
    {
    }

    public class Handler : ICommandHandler<Request>
    {
        private readonly IApplicationOptionsManager _optionsManager;

        public Handler(IApplicationOptionsManager optionsManager)
        {
            _optionsManager = optionsManager;
        }

        public async Task<CommandResponse> HandleAsync(Request request, CancellationToken cancellationToken)
        {
            var _settings1 = _optionsManager.GetOptions<MySettings1>("rodrigo.basniak");
            var _settings2 = _optionsManager.GetOptions<MySettings2>("rodrigo.basniak");

            var dictionary = new Dictionary<string, object>();

            dictionary.Add(_settings1.GetType().Name, _settings1);
            dictionary.Add(_settings2.GetType().Name, _settings2);

            return CommandResponse.Success(dictionary);
        }
    }
} 