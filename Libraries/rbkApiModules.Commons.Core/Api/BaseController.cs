using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core.Utilities.Localization;
using Serilog;
using System.Net;
using System.Text.Json;

namespace rbkApiModules.Commons.Core;

public class BaseController : ControllerBase
{
    private IMapper _mapper;
    private IMemoryCache _cache;
    private IMediator _mediator;
    private ILogger _logger;
    private ILocalizationService _localization;

    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService(typeof(IMediator)) as IMediator;
    protected IMapper Mapper => _mapper ??= HttpContext.RequestServices.GetService(typeof(IMapper)) as IMapper;
    protected IMemoryCache Cache => _cache ??= HttpContext.RequestServices.GetService(typeof(IMemoryCache)) as IMemoryCache;
    protected ILocalizationService Localization => _localization ??= HttpContext.RequestServices.GetService(typeof(ILocalizationService)) as ILocalizationService;
    protected ILogger Logger => _logger;

    public BaseController()
    {
        _logger = Log.Logger.ForContext(this.GetType());
    }

    protected ActionResult<T> HttpResponse<T>(BaseResponse response, string cacheId = null)
    {
        if (response.Status == CommandStatus.Valid)
        {
            T results;

            try
            {
                results = Mapper.Map<T>(response.Result);

                if (cacheId != null)
                {
                    Cache.Set(cacheId, response.Result);
                }

                return Ok(results);
            }
            catch (AutoMapperMappingException ex)
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException is SafeException safeException1)
                    {
                        response.AddHandledError(ex.InnerException.Message);

                        if (safeException1.ShouldBeLogged)
                        {
                            _logger.Information(ex, "Minor exception while mapping results in an endpoint");
                        }

                        return HttpErrorResponse(response);
                    }
                    else
                    {
                        if (ex.InnerException.InnerException != null)
                        {
                            if (ex.InnerException.InnerException is SafeException safeException2)
                            {
                                response.AddHandledError(ex.InnerException.InnerException.Message);

                                if (safeException2.ShouldBeLogged)
                                {
                                    _logger.Information(ex, "Minor exception while mapping results in an endpoint");
                                }

                                return HttpErrorResponse(response);
                            }
                        }
                    }
                }

                response.AddUnhandledError(_localization.LocalizeString(CommentMessages.Errors.InternalServerError));

                _logger.Fatal(ex, "AutoMapper exception was thrown while mapping results in an endpoint");

                return HttpErrorResponse(response);
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, "Exception was thrown while mapping results in an endpoint");

                response.AddUnhandledError(_localization.LocalizeString(CommentMessages.Errors.InternalServerError));

                return HttpErrorResponse(response);
            }
        }
        else
        {
            return HttpErrorResponse(response);
        }
    } 

    protected ActionResult HttpResponse(BaseResponse response, string cacheId = null)
    {
        if (response.Status == CommandStatus.Valid)
        {
            if (response.Result is FileResult dto)
            {
                HttpContext.Items.Add("response-size", dto.FileStream.Length);
                return File(dto.FileStream, dto.ContentType, dto.FileName);
            }
            // TODO: check if the generic is working
            else if (response.Result != null && !(response.Result.GetType() == typeof(BaseEntity)))
            {
                if (cacheId != null)
                {
                    Cache.Set(cacheId, response.Result);
                }

                return Ok(response.Result);
            }
            else
            {
                return Ok();
            }
        }
        else
        {
            return HttpErrorResponse(response);
        }
    }

    private ActionResult HttpErrorResponse(BaseResponse response)
    {
        var messages = response.Errors.Select(x => x.Message).ToArray();

        switch (response.Status)
        {
            case CommandStatus.HasHandledError:
                if (response.Errors.Any(x => x.Code == ValidationErrorCodes.UNAUTHORIZED))
                {
                    return new ContentResult()
                    {
                        Content = JsonSerializer.Serialize(messages),
                        StatusCode = (int)HttpStatusCode.Forbidden
                    };
                }
                else if (response.Errors.Any(x => x.Code == ValidationErrorCodes.UNAUTHORIZED))
                {
                    return NotFound(messages);
                }
                else
                {
                    return BadRequest(messages);
                }
            case CommandStatus.HasUnhandledError:
                if (HttpContext.Request.Method.ToUpper() == "GET")
                {
                    return StatusCode(500, messages);
                }
                else
                {
                    return BadRequest(messages);
                }
            default:
                throw new ArgumentException("Unknow error status code.");
        }
    }

    protected void InvalidateCache(string cacheId)
    {
        Cache.Remove(cacheId);
    }
}