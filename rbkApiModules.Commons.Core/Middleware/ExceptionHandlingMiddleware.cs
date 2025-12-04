using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using rbkApiModules.Commons.Core.Helpers;
using System.Text.Json;

namespace rbkApiModules.Commons.Core;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (InternalException ex)
        {
            if (ex.StatusCode == 400)
            {
                _logger.LogWarning(ex, "An expected issue occurred during request normal execution");   
            }
            else if (ex.StatusCode == 500)
            {
                _logger.LogError(ex, "An expected error occurred during request normal execution");   
            }
            else
            {
                _logger.LogError(ex, "An expected exception occurred but it has an unsuported status code: {statusCode}", ex.StatusCode);   
            }

            var problem = new ProblemDetails
            {
                Status = ex.StatusCode,
                Title = "A handled error occurred",
                Detail = ex.Message,
                Instance = context.Request.Path
            };

            context.Response.StatusCode = ex.StatusCode;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
        catch (InternalValidationException ex)
        {
            _logger.LogError(ex, "An unexpected error occurred during request validation");   

            var problem = new ValidationProblemDetails(ex.Summary)
            {
                Status = 400,
                Title = "Request validation failed",
                Detail = ex.Message,
                Instance = context.Request.Path
            };

            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "An unexpected error occurred outside the request flow");   

            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred",
                Detail = "Unexpected server error",
                Instance = context.Request.Path,
                
            };

            if (TestingEnvironmentChecker.IsTestingEnvironment)
            {
                problem.Extensions.Add("exception", ex.ToBetterString());
            }

            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
    }
}
