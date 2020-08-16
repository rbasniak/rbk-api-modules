using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace rbkApiModules.Infrastructure
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

        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
        protected IMapper Mapper => _mapper ??= HttpContext.RequestServices.GetService<IMapper>();
        protected IMemoryCache Cache => _cache ??= HttpContext.RequestServices.GetService<IMemoryCache>();

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
        protected ActionResult<T> HttpResponse<T>(BaseResponse response)
        {
            if (response.Status == CommandStatus.Valid)
            {
                return Ok(Mapper.Map<T>(response.Result));
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
        protected ActionResult HttpResponse(BaseResponse response)
        {
            if (response.Status == CommandStatus.Valid)
            {
                return Ok();
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
    }
}
