using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using rbkApiModules.Diagnostics.Commons;
using rbkApiModules.Infrastructure.MediatR.Core;
using rbkApiModules.Infrastructure.Models;
using System;

namespace rbkApiModules.Infrastructure.Api
{
    /// <summary>
    /// Classe de controller da qual os outros controllers do projeto devem herdar
    /// </summary>
    [Authorize]
    public class BaseController : ControllerBase
    {
        private IMapper _mapper;
        private IMemoryCache _cache;
        private IMediator _mediator;
        private IDiagnosticsModuleStore _diagnosticsStore;

        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService(typeof(IMediator)) as IMediator;
        protected IMapper Mapper => _mapper ??= HttpContext.RequestServices.GetService(typeof(IMapper)) as IMapper;
        protected IMemoryCache Cache => _cache ??= HttpContext.RequestServices.GetService(typeof(IMemoryCache)) as IMemoryCache;
        protected IDiagnosticsModuleStore DiagnosticsStore => _diagnosticsStore ??= HttpContext.RequestServices.GetService(typeof(IDiagnosticsModuleStore)) as IDiagnosticsModuleStore;

        public BaseController()
        {
        }

        /// <summary>
        /// Método para retornar um ActionResult de acordo com o status do 
        /// comando retornado pelo handler
        /// Usar a versão genérica apenas quando o comando retornar uma entidade no payload do resultado
        /// </summary>
        /// <typeparam name="T">Tipo de retorno (apenas para aparecer corretamente no Swagger)</typeparam>
        /// <param name="response">Resultada da execução de um request do MediatR</param>
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
                        if (ex.InnerException is SafeException || ex.InnerException is KindaSafeException)
                        {
                            response.AddHandledError(ex.InnerException.Message);

                            if (DiagnosticsStore != null && ex.InnerException is KindaSafeException)
                            {
                                var exceptionData = new DiagnosticsEntry(HttpContext, "BaseController HttpResponse", ex, null);
                                DiagnosticsStore.StoreData(exceptionData);
                            }

                            return HttpErrorResponse(response);
                        }
                        else
                        {
                            if (ex.InnerException.InnerException != null)
                            {
                                if (ex.InnerException.InnerException is SafeException || ex.InnerException.InnerException is KindaSafeException)
                                {
                                    response.AddHandledError(ex.InnerException.InnerException.Message);

                                    if (DiagnosticsStore != null && ex.InnerException.InnerException is KindaSafeException)
                                    {
                                        var exceptionData = new DiagnosticsEntry(HttpContext, "BaseController HttpResponse", ex, null);
                                        DiagnosticsStore.StoreData(exceptionData);
                                    }

                                    return HttpErrorResponse(response);
                                }
                            }
                        }
                    }

                    response.AddUnhandledError(ex.Message);
                    return HttpErrorResponse(response);
                }
                catch (Exception ex)
                {
                    response.AddUnhandledError(ex.Message);
                    return HttpErrorResponse(response);
                }
            }
            else
            {
                return HttpErrorResponse(response);
            }
        }

        /// <summary>
        /// Método para retornar um resultado de acordo com o status do 
        /// comando retornado pelo handler
        /// Usar a versão não genérica apenas quando o comando não retornar uma entidade no payload do resultado
        /// </summary>
        /// <param name="response">Resultada da execução de um request do MediatR</param>
        protected ActionResult HttpResponse(BaseResponse response, string cacheId = null)
        {
            if (response.Status == CommandStatus.Valid)
            {
                if (response.Result is Models.FileResult dto)
                {
                    HttpContext.Items.Add("response-size", dto.FileStream.Length);
                    return File(dto.FileStream, dto.ContentType, dto.FileName);
                }
                else if (response.Result != null && !(response.Result is BaseEntity))
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
            switch (response.Status)
            {
                case CommandStatus.HasHandledError:
                    return BadRequest(response.Errors);
                case CommandStatus.HasUnhandledError:
                    return StatusCode(500, response.Errors);
                default:
                    throw new ArgumentException("Unknow error status code.");
            }
        }

        protected void InvalidateCache(string cacheId)
        {
            Cache.Remove(cacheId);
        }
    }
}
