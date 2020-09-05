using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Authentication;
using rbkApiModules.Infrastructure;
using rbkApiModules.Infrastructure.Api.Controllers;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AspNetCoreApiTemplate.Api
{
    /// <summary>
    /// Controller para acesso das funcionalidades de autenticação
    /// </summary>
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class AuthController : BaseController
    {
        /// <summary>
        /// Login de usuário
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<JwtResponse>> Login([FromBody]UserLogin.Command data)
        {
            try
            {
                var result = await Mediator.Send(data);

                if (result.Result == null) return new ContentResult()
                {
                    Content = result.Errors.Any() ? result.Errors.First() : "Usuário sem permissões para esta aplicação",
                    StatusCode = (int)HttpStatusCode.Unauthorized
                };

                if (result.IsValid)
                {
                    return HttpResponse<JwtResponse>(result);
                }
                else
                {
                    return new ContentResult()
                    {
                        Content = result.Errors.First(), // Sempre terá apenas um erro, e o Content é do tipo string e não string[]
                        StatusCode = (int)HttpStatusCode.Unauthorized
                    };
                };
            }
            catch (Exception ex)
            {
                return new ContentResult()
                {
                    Content = ex.Message, // Sempre terá apenas um erro, e o Content é do tipo string e não string[]
                    StatusCode = (int)HttpStatusCode.Unauthorized
                };
            }
        }

        /// <summary>
        /// Renova um token expirado
        /// </summary>
        [HttpPost("refresh-token")]
        public async Task<ActionResult<JwtResponse>> Post([FromBody] RenewAccessToken.Command data)
        {
            var result = await Mediator.Send(data);

            if (result.IsValid)
            {
                return HttpResponse<JwtResponse>(result);
            }
            else
            {
                return new ContentResult()
                {
                    Content = result.Errors.First(), // Sempre terá apenas um erro, e o Content é do tipo string e não string[]
                    StatusCode = (int)HttpStatusCode.Unauthorized
                };
            };
        }
    }
}
