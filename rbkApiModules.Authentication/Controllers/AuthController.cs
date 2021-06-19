using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.CodeGeneration.Commons;
using rbkApiModules.Infrastructure.Api;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace rbkApiModules.Authentication
{
    /// <summary>
    /// Controller para acesso das funcionalidades de autenticação
    /// </summary>
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class AuthController : BaseController
    {
        private readonly AuthenticationMailConfiguration _MailConfig;

        public AuthController(AuthenticationMailConfiguration config) : base()
        {
            _MailConfig = config;
        }

        /// <summary>
        /// Login de usuário
        /// </summary>
        [IgnoreOnCodeGeneration]
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
        [IgnoreOnCodeGeneration]
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

        /// <summary>
        /// Envia um e-mail contendo o link para troca de senha
        /// </summary>
        [HttpPost]
        [Route("reset-password")]
        public async Task<ActionResult> SendResetPasswordEmail(ResetPassword.Command data)
        {
            return HttpResponse(await Mediator.Send(data));
        }

        /// <summary>
        /// Redefine a senha do usuário dado um código de redefinição
        /// </summary>
        [HttpPost]
        [Route("redefine-password")]
        public async Task<ActionResult> RedefinePassword(RedefinePassword.Command data)
        {
            return HttpResponse(await Mediator.Send(data));
        }

        /// <summary>
        /// Reenvia o e-mail de confirmação para o usuário
        /// </summary>
        [HttpPost("resend-confirmation")]
        public async Task<ActionResult> ResendEmailConfirmation(ResendEmailConfirmation.Command data)
        {
            return HttpResponse(await Mediator.Send(data));
        }

        /// <summary>
        /// Confirma o e-mail de um usuário
        /// </summary>
        [IgnoreOnCodeGeneration]
        [HttpGet("confirm-email")]
        public async Task<ActionResult> ConfirmEmail(string email, string code)
        {
            var response = await Mediator.Send(new ConfirmUserEmail.Command() { Email = email, ActivationCode = code });

            if (response.IsValid)
            {
                return Redirect(_MailConfig.ConfirmationSuccessUrl);
            }
            else
            {
                return Redirect(_MailConfig.ConfirmationFailedUrl);
            }
        }
    }
}
